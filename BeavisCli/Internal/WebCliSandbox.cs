using BeavisCli.Internal.DefaultServices;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BeavisCli.Internal
{
    internal class WebCliSandbox
    {
        private readonly ILogger<WebCliSandbox> _logger;
        private readonly IAuthorizationHandler _authorization;
        private readonly IUnauthorizedHandler _unauthorized;
        private readonly WebCliOptions _options;

        public WebCliSandbox(ILoggerFactory loggerFactory, 
                             IAuthorizationHandler authorization, 
                             IUnauthorizedHandler unauthorized, 
                             IOptions<WebCliOptions> options)
        {
            _logger = loggerFactory.CreateLogger<WebCliSandbox>();
            _authorization = authorization;
            _unauthorized = unauthorized;
            _options = options.Value;
        }

        public async Task ExecuteAsync(WebCliRequest request, WebCliResponse response, HttpContext httpContext)
        {
            try
            {
                _logger.LogDebug($"Started to process a request with the input '{request.Input}'.");

                // application name entered by the user
                string name = request.GetApplicationName();

                _logger.LogDebug($"Searching an application by the name '{name}'.");

                // search application by name, throws WebCliSandboxException if not found!
                WebCliApplication application = GetApplication(name, httpContext);

                _logger.LogDebug($"Found an application '{application.GetType().FullName}' that matches the name '{name}'.");

                WebCliApplicationInfo info = application.GetInfo();

                // these TextWriters are for writing console out and error messages, just
                // like Console.Out and Console.Error
                TextWriter outWriter = new ResponseMessageTextWriter(response.WriteInformation);
                TextWriter errorWriter = new ResponseMessageTextWriter(response.WriteError);

                CommandLineApplication cli = new CommandLineApplication
                {
                    Name = info.Name,
                    FullName = info.Name,
                    Description = info.Description,
                    Out = outWriter,
                    Error = errorWriter
                };                
                cli.HelpOption("-?|-h|--help");

                WebCliContext context = new WebCliContext(cli, httpContext, request, response);

                // check authorization
                bool authorized = IsAuthorized(application, context);

                if (authorized)
                {
                    await application.ExecuteAsync(context);
                }
                else
                {
                    // handle unauthorized attempts
                    await _unauthorized.OnUnauthorizedAsync(context);
                }               
            }
            catch (WebCliSandboxException e)
            {
                _logger.LogDebug($"An error occurred while searching an application by using the input '{request.Input}'.", e);
                response.WriteError(e);
            }
            catch (CommandParsingException e)
            {
                _logger.LogDebug($"An error occurred while parsing the input '{request.Input}'.", e);
                response.WriteError(e);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while processing the request with the input '{request.Input}'.", e);

                if (_options.DisplayExceptions)
                {
                    response.WriteError(e, true);
                }
                else
                {
                    response.WriteError("An error occurred, please check your application logs for more details.");
                }
            }
        }

        public WebCliApplication GetApplication(string name, HttpContext httpContext)
        {
            int count = 0;

            WebCliApplication result = null;

            foreach (WebCliApplication application in GetApplications(httpContext))
            {
                WebCliApplicationInfo info = application.GetInfo();

                if (info.Name == name)
                {
                    count++;
                    result = application;
                }

                if (count > 1)
                {
                    throw new WebCliSandboxException($"Found more than one application with name a '{name}'. Application names must me unique.");
                }
            }

            if (count == 0)
            {
                throw new WebCliSandboxException($"{name} is not a valid application.{Environment.NewLine}Usage 'help' to get list of applications.");
            }

            return result;
        }

        public IEnumerable<WebCliApplication> GetApplications(HttpContext httpContext)
        {
            foreach (WebCliApplication application in httpContext.GetWebCliApplications())
            {
                WebCliApplicationInfo info = application.GetInfo();
                if (info != null)
                {
                    yield return application;
                }
            }
        }

        /// <summary>
        /// Checks if the application execution is authorized.
        /// </summary>
        private bool IsAuthorized(WebCliApplication application, WebCliContext context)
        {
            bool authorized = _authorization.IsAuthorized(application, context);

            bool externalHandler = !(_authorization is DefaultAuthorizationHandler);

            bool builtIn = application.IsBuiltIn();
           
            if (externalHandler)
            {
                _logger.LogInformation($"The authorization status returned by the current {nameof(IAuthorizationHandler)} implementation '{_authorization.GetType().FullName}' is {authorized}.");
            }

            if (authorized)
            {
                authorized = application.IsAuthorized(context);

                if (!builtIn)
                {
                    _logger.LogInformation($"The authorization status returned by the application '{application.GetType().FullName}' is {authorized}.");
                }
            }

            if (!builtIn || externalHandler)
            {
                if (authorized)
                {
                    _logger.LogInformation($"The application '{application.GetType().FullName}' execution is authorized.");
                }
                else
                {
                    _logger.LogInformation($"The application '{application.GetType().FullName}' execution is unauthorized.");
                }
            }

            return authorized;
        }        
    }
}