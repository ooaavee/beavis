using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BeavisCli.Internal
{
    internal class WebCliSandbox
    {
        private readonly IAuthorizationHandler _authorizationHandler;
        private readonly IUnauthorizedHandler _unauthorizedHandler;
        private readonly WebCliOptions _options;

        public WebCliSandbox(IAuthorizationHandler authorizationHandler, IUnauthorizedHandler unauthorizedHandler, IOptions<WebCliOptions> options)
        {
            _authorizationHandler = authorizationHandler;
            _unauthorizedHandler = unauthorizedHandler;
            _options = options.Value;
        }

        public async Task ExecuteAsync(WebCliRequest request, WebCliResponse response, HttpContext httpContext)
        {
            try
            {
                string name = request.GetApplicationName();

                WebCliApplication application = GetApplication(name, httpContext);

                CommandLineApplication cli = new CommandLineApplication
                {
                    Name = application.Meta().Name,
                    FullName = application.Meta().Name,
                    Description = application.Meta().Description,
                    Out = new ResponseMessageTextWriter(response.WriteInformation),
                    Error = new ResponseMessageTextWriter(response.WriteError)
                };

                cli.HelpOption("-?|-h|--help");

                WebCliApplicationHost host = new WebCliApplicationHost(cli);

                WebCliContext context = new WebCliContext(request, response, httpContext, host);

                if (IsAuthorized(application, context))
                {
                    await ExecuteApplicationAsync(application, context);
                }
                else
                {
                    await HandleUnauthorizedAsync(context);
                }
            }
            catch (WebCliSandboxException e)
            {
                response.WriteError(e);
            }
            catch (CommandParsingException e)
            {
                response.WriteError(e);
            }
            catch (Exception e)
            {
                if (_options.DisplayExceptions)
                {
                    response.WriteError(e, true);
                }
                else
                {
                    response.WriteError("An error occurred. Please check your application logs for more details.");
                }
            }
        }

        public WebCliApplication GetApplication(string name, HttpContext httpContext)
        {
            int matchCount = 0;

            WebCliApplication result = null;

            foreach (WebCliApplication application in GetApplications(httpContext))
            {
                if (application.Meta().Name == name)
                {
                    matchCount++;
                    result = application;
                }

                if (matchCount > 1)
                {
                    throw new WebCliSandboxException($"Found more than one application with name a '{name}'. Application names must me unique.");
                }
            }

            if (matchCount == 0)
            {
                if (_options.EnableDefaultApplications)
                {
                    throw new WebCliSandboxException($"{name} is not a valid application.{Environment.NewLine}Usage 'help' to get list of applications.");
                }

                throw new WebCliSandboxException($"{name} is not a valid application.");
            }

            return result;
        }

        public IEnumerable<WebCliApplication> GetApplications(HttpContext httpContext)
        {
            foreach (WebCliApplication application in httpContext.RequestServices.GetServices<WebCliApplication>())
            {
                if (application.Meta() != null)
                {
                    yield return application;
                }
            }
        }

        public bool IsDefault(WebCliApplication application)
        {
            return application.GetType().Assembly.Equals(GetType().Assembly);
        }

        private bool IsAuthorized(WebCliApplication application, WebCliContext context)
        {
            bool authorized = _authorizationHandler.IsAuthorized(application, context);

            if (authorized)
            {
                authorized = application.IsAuthorized(context);
            }

            return authorized;
        }

        private async Task ExecuteApplicationAsync(WebCliApplication application, WebCliContext context)
        {
            await application.ExecuteAsync(context);
        }

        private async Task HandleUnauthorizedAsync(WebCliContext context)
        {
            await _unauthorizedHandler.OnUnauthorizedAsync(context);
        }
    }
}