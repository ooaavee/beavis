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

                // command name entered by the user
                string name = request.GetCommandName();

                _logger.LogDebug($"Searching an command by the name '{name}'.");

                // search command by name, throws WebCliSandboxException if not found!
                WebCliCommand cmd = GetCommand(name, httpContext);

                _logger.LogDebug($"Found a command '{cmd.GetType().FullName}' that matches the name '{name}'.");

                // these TextWriters are for writing console out and error messages, just
                // like Console.Out and Console.Error
                TextWriter outWriter = new ResponseMessageTextWriter(response.WriteInformation);
                TextWriter errorWriter = new ResponseMessageTextWriter(response.WriteError);

                CommandLineApplication processor = new CommandLineApplication
                {
                    Name = cmd.Info.Name,
                    FullName = cmd.Info.Name,
                    Description = cmd.Info.Description,
                    Out = outWriter,
                    Error = errorWriter
                };                
                processor.HelpOption("-?|-h|--help");

                WebCliContext context = new WebCliContext(processor, httpContext, request, response);

                // check authorization
                bool authorized = IsAuthorized(cmd, context);

                if (authorized)
                {
                    await cmd.ExecuteAsync(context);
                }
                else
                {
                    // handle unauthorized attempts
                    await _unauthorized.OnUnauthorizedAsync(context);
                }               
            }
            catch (WebCliSandboxException e)
            {
                _logger.LogDebug($"An error occurred while searching a command by using the input '{request.Input}'.", e);
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

        public WebCliCommand GetCommand(string name, HttpContext httpContext)
        {
            int count = 0;

            WebCliCommand result = null;

            foreach (WebCliCommand cmd in GetCommands(httpContext))
            {
                if (cmd.Info.Name == name)
                {
                    count++;
                    result = cmd;
                }

                if (count > 1)
                {
                    throw new WebCliSandboxException($"Found more than one command with name a '{name}'. Command names must me unique.");
                }
            }

            if (count == 0)
            {
                throw new WebCliSandboxException($"{name} is not a valid command.{Environment.NewLine}Usage 'help' to get list of commands.");
            }

            return result;
        }

        public IEnumerable<WebCliCommand> GetCommands(HttpContext httpContext)
        {
            foreach (WebCliCommand cmd in httpContext.GetWebCliCommands())
            {
                if (cmd.IsValid)
                {
                    yield return cmd;
                }
            }
        }

        /// <summary>
        /// Checks if the command execution is authorized.
        /// </summary>
        private bool IsAuthorized(WebCliCommand cmd, WebCliContext context)
        {
            bool authorized = _authorization.IsAuthorized(cmd, context);

            bool externalHandler = !(_authorization is DefaultAuthorizationHandler);
           
            if (externalHandler)
            {
                _logger.LogInformation($"The authorization status returned by the current {nameof(IAuthorizationHandler)} implementation '{_authorization.GetType().FullName}' is {authorized}.");
            }

            if (authorized)
            {
                authorized = cmd.IsAuthorized(context);

                if (!cmd.IsBuiltIn)
                {
                    _logger.LogInformation($"The authorization status returned by the command '{cmd.GetType().FullName}' is {authorized}.");
                }
            }

            if (!cmd.IsBuiltIn || externalHandler)
            {
                if (authorized)
                {
                    _logger.LogInformation($"The command '{cmd.GetType().FullName}' execution is authorized.");
                }
                else
                {
                    _logger.LogInformation($"The command '{cmd.GetType().FullName}' execution is unauthorized.");
                }
            }

            return authorized;
        }        
    }
}