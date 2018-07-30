using BeavisCli.Internal;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BeavisCli.Services
{
    public class RequestExecutor : IRequestExecutor
    {
        private readonly ILogger<RequestExecutor> _logger;
        private readonly ICommandProvider _commands;
        private readonly IAuthorizationHandler _authorization;
        private readonly IUnauthorizedHandler _unauthorized;
        private readonly BeavisCliOptions _options;

        public RequestExecutor(ILoggerFactory loggerFactory,
                               ICommandProvider commands, 
                               IAuthorizationHandler authorization,
                               IUnauthorizedHandler unauthorized,
                               IOptions<BeavisCliOptions> options)
        {
            _logger = loggerFactory.CreateLogger<RequestExecutor>();
            _commands = commands;
            _authorization = authorization;
            _unauthorized = unauthorized;
            _options = options.Value;
        }

        public async Task ExecuteAsync(Request request, Response response, HttpContext httpContext)
        {         
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            try
            {
                _logger.LogDebug($"Started to process a request with the input '{request.Input}'.");

                // command name entered by the user
                string name = request.GetCommandName();

                _logger.LogDebug($"Searching an command by the name '{name}'.");

                // find the command
                Command cmd;
                try
                {
                    cmd = _commands.GetCommand(name, httpContext);
                }
                catch (Exception e)
                {
                    _logger.LogDebug($"An error occurred while searching a command by using the input '{request.Input}'.", e);
                    response.WriteError(e);
                    return;
                }

                _logger.LogDebug($"Found a command '{cmd.GetType().FullName}' that matches the name '{name}'.");

                // these TextWriters are for writing console out and error messages, just
                // like Console.Out and Console.Error
                TextWriter outWriter = new ResponseMessageTextWriter(response.WriteInformation);
                TextWriter errorWriter = new ResponseMessageTextWriter(response.WriteError);

                CommandLineApplication processor = new CommandLineApplication
                {
                    Name = cmd.Info.Name,
                    FullName = cmd.Info.FullName,
                    Description = cmd.Info.Description,
                    Out = outWriter,
                    Error = errorWriter
                };
                processor.HelpOption("-?|-h|--help");

                CommandContext context = new CommandContext(processor, httpContext, request, response);

                // check authorization
                bool authorized = IsAuthorized(cmd, context);

                if (authorized)
                {
                    // execute command
                    await cmd.ExecuteAsync(context);
                }
                else
                {
                    // handle unauthorized execution attempts
                    await _unauthorized.OnUnauthorizedAsync(cmd, context);
                }
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

        /// <summary>
        /// Checks if the command execution is authorized.
        /// </summary>
        protected virtual bool IsAuthorized(Command cmd, CommandContext context)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            bool authorized = _authorization.IsAuthorized(cmd, context);

            bool externalHandler = !(_authorization is AuthorizationHandler);

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
