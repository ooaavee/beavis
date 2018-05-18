using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Internal
{
    internal class WebCliSandbox
    {
        private readonly IUnauthorizedHandler _unauthorized;
        private readonly WebCliOptions _options;

        public WebCliSandbox(IUnauthorizedHandler unauthorized, IOptions<WebCliOptions> options)
        {
            _unauthorized = unauthorized;
            _options = options.Value;
        }

        public async Task ExecuteAsync(WebCliRequest request, WebCliResponse response, HttpContext httpContext)
        {
            try
            {
                string name = ParseApplicationName(request);

                WebCliApplication application = GetApplicaton(name, httpContext);

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
                    _unauthorized.OnUnauthorized(context);
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
                response.WriteError(ex, true);
            }
        }

        public WebCliApplication GetApplicaton(string name, HttpContext httpContext)
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
                if (_options.EnableDefaultApplications)
                {
                    throw new BeavisCliSandboxException($"{name} is not a valid application.{Environment.NewLine}Usage 'help' to get list of applications.");
                }

                throw new BeavisCliSandboxException($"{name} is not a valid application.");
            }

            return result;
        }

        public IEnumerable<WebCliApplication> GetApplications(HttpContext httpContext)
        {
            foreach (WebCliApplication application in httpContext.RequestServices.GetServices<WebCliApplication>())
            {
                if (application.TryInitialize())
                {
                    yield return application;
                }
            }
        }

        public bool IsDefault(WebCliApplication app)
        {
            return app.GetType().Assembly.Equals(GetType().Assembly);
        }

        /// <summary>
        /// Parses the application name from the request.
        /// </summary>
        public string ParseApplicationName(WebCliRequest request)
        {
            string value = null;
            if (request.Input != null)
            {
                value = request.Input.Trim();
                var tokens = value.Split(' ');
                if (tokens.Any())
                {
                    value = tokens.First();
                }
            }
            return value;
        }

        /// <summary>
        /// Parses application arguments from the request.
        /// </summary>
        public string[] ParseApplicationArgs(WebCliRequest request)
        {
            var args = new List<string>();
            if (request.Input != null)
            {
                var tokens = request.Input.Trim().Split(' ');
                if (tokens.Length > 1)
                {
                    for (var i = 1; i <= tokens.Length - 1; i++)
                    {
                        var arg = tokens[i].Trim();
                        if (arg.Length > 0)
                        {
                            args.Add(arg);
                        }
                    }
                }
            }
            return args.ToArray();
        }
    }
}