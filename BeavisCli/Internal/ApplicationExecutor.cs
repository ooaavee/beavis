using System;
using System.Threading.Tasks;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Internal
{
    internal class ApplicationExecutor
    {
        private readonly ApplicationProvider _provider;
        private readonly UnauthorizedApplicationExecutionAttemptHandler _unauthorized;

        public ApplicationExecutor(ApplicationProvider provider, UnauthorizedApplicationExecutionAttemptHandler unauthorized)
        {
            _provider = provider;
            _unauthorized = unauthorized;
        }

        public async Task HandleAsync(ApplicationExecutionRequest request, ApplicationExecutionResponse response, HttpContext httpContext)
        {
            try
            {
                string name = request.GetApplicationName();

                AbstractApplication application = _provider.FindApplicaton(name, httpContext);

                ApplicationInfo info = application.GetInfo();

                CommandLineApplication cli = new CommandLineApplication
                {
                    Name = info.Name,
                    FullName = info.Name,
                    Description = info.Description,
                    Out = response.CreateTextWriterForInformationMessages(),
                    Error = response.CreateTextWriterForErrorMessages()
                };

                cli.HelpOption("-?|-h|--help");

                ICommandLineApplication host = new DefaultCommandLineApplication(cli);

                ApplicationExecutionContext context = new ApplicationExecutionContext(request, response, httpContext, host, info);

                AuthorizeResult authorization = await application.OnAuthorize(context);

                if (authorization.IsAuthorized())
                {
                    await application.ExecuteAsync(context);
                }
                else
                {
                    _unauthorized.HandleUnauthorizedApplicationExecution(context);
                }
            }
            catch (ApplicationProviderException ex)
            {
                response.WriteError(ex);
            }
            catch (CommandParsingException ex)
            {
                response.WriteError(ex);
            }
            catch (Exception ex)
            {
                response.WriteError(ex, returnStackTrace: true);
            }

        }
    }
}