using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeavisCli.Internal
{
    internal class WebCliSandbox
    {
        private readonly WebCliOptions _options;

        public WebCliSandbox(IOptions<WebCliOptions> options)
        {
            _options = options.Value;
        }

        public async Task ExecuteAsync(WebCliRequest request, WebCliResponse response, HttpContext httpContext)
        {
            try
            {
                string name = request.GetApplicationName();

                WebCliApplication application = FindApplicaton(name, httpContext);

                CommandLineApplication cli = new CommandLineApplication
                {
                    Name = application.Name,
                    FullName = application.Name,
                    Description = application.Description,
                    Out = new ResponseMessageTextWriter(response.WriteInformation),
                    Error = new ResponseMessageTextWriter(response.WriteError)
                };

                cli.HelpOption("-?|-h|--help");

                WebCliApplicationHost host = new WebCliApplicationHost(cli);

                WebCliContext context = new WebCliContext(request, response, httpContext, host);

                if (application.IsAuthorized(context))
                {
                    await application.ExecuteAsync(context);
                }
                else
                {
                    if (_options.UnauthorizedHandler != null)
                    {
                        _options.UnauthorizedHandler.HandleUnauthorizedApplicationExecution(context);
                    }
                }
            }
            catch (BeavisCliSandboxException ex)
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

        public WebCliApplication FindApplicaton(string name, HttpContext httpContext)
        {
            int matchCount = 0;

            WebCliApplication result = null;

            foreach (WebCliApplication application in GetApplications(httpContext))
            {
                if (application.Name == name)
                {
                    matchCount++;
                    result = application;
                }

                if (matchCount > 1)
                {
                    throw new BeavisCliSandboxException($"Found more than one application with name '{name}'. Application names must me unique.");
                }
            }

            if (matchCount == 0)
            {
                if (_options.UseDefaultApplications)
                {
                    throw new BeavisCliSandboxException($"{name} is not a valid application.{Environment.NewLine}Usage 'help' to get list of applications.");
                }
                throw new BeavisCliSandboxException($"{name} is not a valid application.");
            }

            return result;
        }

        public IEnumerable<WebCliApplication> GetApplications(HttpContext httpContext)
        {
            IEnumerable<WebCliApplication> applications = httpContext.RequestServices.GetServices<WebCliApplication>();
            return applications;
        }
    }
}