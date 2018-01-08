using System;
using System.Threading.Tasks;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Internal
{
    internal class RequestHandler
    {
        private readonly ApplicationSpace _space;

        public RequestHandler(ApplicationSpace space)
        {
            _space = space;
        }

        public async Task HandleAsync(CliRequest request, HttpContext httpContext)
        {
            CliContext context = new CliContext(request, new CliResponse(), httpContext);

            try
            {
                string name = request.ParseApplicationName();

                AbstractApplication app = _space.FindApplicaton(name);

                ICommandLineApplication cli = app.CreateCommandLineApplication(context);

                await app.ExecuteAsync(cli, context);
            }
            catch (ApplicationSpaceException ex)
            {
                using (context.Response.BeginInteraction())
                {
                    context.Response.WriteError(ex);
                }
            }
            catch (CommandParsingException ex)
            {
                using (context.Response.BeginInteraction())
                {
                    context.Response.WriteError(ex);
                }
            }
            catch (Exception ex)
            {
                //
                // TODO: Pitäisikö tässä palauttaa käyttöliittymälle koko stack trace
                //
                using (context.Response.BeginInteraction())
                {
                    context.Response.WriteError(ex);
                }
            }

        }
    }
}