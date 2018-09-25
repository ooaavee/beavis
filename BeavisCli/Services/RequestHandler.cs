using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BeavisCli.Services
{
    public class RequestHandler : IRequestHandler
    {
        private readonly ILogger<RequestHandler> _logger;
        private readonly BeavisCliOptions _options;

        public RequestHandler(ILoggerFactory loggerFactory, IOptions<BeavisCliOptions> options)
        {
            _logger = loggerFactory.CreateLogger<RequestHandler>();
            _options = options.Value;
        }

        public async Task<Response> HandleAsync(Request request, HttpContext httpContext)
        {
            // required services
            ICommandProvider commands = httpContext.RequestServices.GetRequiredService<ICommandProvider>();
            IUnauthorizedHandler unauthorized = httpContext.RequestServices.GetRequiredService<IUnauthorizedHandler>();

            Response response = new Response();

            CommandContext context = null;

            try
            {
                _logger.LogDebug($"Started to process a request '{request.Input}'.");
            
                // command name entered by the user
                string name = request.GetCommandName();

                _logger.LogDebug($"Searching a command by the name '{name}'.");

                // find the command by using the ICommandProvider service
                ICommand cmd;
                try
                {
                    cmd = commands.GetCommand(name, httpContext);
                }
                catch (Exception e)
                {
                    _logger.LogDebug($"An error occurred while searching a command by using the input '{request.Input}'.", e);
                    response.Messages.Add(ResponseMessage.Error(e.Message));
                    return response;
                }

                _logger.LogDebug($"Found a command '{cmd.GetType().FullName}' that matches the name '{name}'.");

                CommandInfo info = CommandInfo.Get(cmd);

                context = new CommandContext(httpContext, request, response, info, cmd);

                CommandBuilder builder = new CommandBuilder(context);

                // check authorization
                bool authorized = IsAuthorized(context);

                if (authorized)
                {
                    // execute command
                    await cmd.ExecuteAsync(builder, context);
                }
                else
                {
                    // handle unauthorized execution attempts
                    await unauthorized.OnUnauthorizedAsync(context);
                }
            }
            catch (CommandParsingException e)
            {
                _logger.LogDebug($"An error occurred while parsing the input '{request.Input}'.", e);

                if (context != null)
                {
                    context.WriteError(e);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while processing the request with the input '{request.Input}'.", e);

                if (context != null)
                {
                    if (_options.DisplayExceptions)
                    {
                        context.WriteError(e, true);
                    }
                    else
                    {
                        context.WriteError("An error occurred, please check your application logs for more details.");
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Checks if the command execution is authorized.
        /// </summary>
        protected virtual bool IsAuthorized(CommandContext context)
        {
            ICommandExecutionEnvironment environment = context.HttpContext.RequestServices.GetRequiredService<ICommandExecutionEnvironment>();

            bool authorized = environment.IsAuthorized(context);

            _logger.LogInformation($"The authorization status for the command '{context.Command.GetType().FullName}' is {authorized}.");

            return authorized;
        }
    }
}
