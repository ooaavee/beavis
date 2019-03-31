using BeavisCli.Extensions;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BeavisCli.Services.Defaults
{
    public class RequestHandler : IRequestHandler
    {
        private readonly ILogger<RequestHandler> _logger;
        private readonly BeavisCliOptions _options;

        public RequestHandler(ILogger<RequestHandler> logger, IOptions<BeavisCliOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<Response> HandleAsync(Request request, HttpContext context)
        {
            // required services
            ICommandProvider commands = context.RequestServices.GetRequiredService<ICommandProvider>();
            IUnauthorizedHandler unauthorized = context.RequestServices.GetRequiredService<IUnauthorizedHandler>();

            Response response = new Response();

            CommandContext ctx = null;

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
                    cmd = commands.Find(name, context);
                }
                catch (Exception e)
                {
                    _logger.LogDebug($"An error occurred while searching a command by using the input '{request.Input}'.", e);
                    response.Messages.Add(ResponseMessage.Error(e.Message));
                    return response;
                }

                _logger.LogDebug($"Found a command '{cmd.GetType().FullName}' that matches the name '{name}'.");

                CommandInfo info = cmd.GetType().GetCommandInfo();

                ctx = new CommandContext(context, request, response, info, cmd);

                if (await IsAuthorizedAsync(ctx))
                {
                    try
                    {
                        await ExecuteCommandAsync(ctx);
                    }
                    catch (UnauthorizedException)
                    {
                        // ...nope, command says unauthorized
                        await unauthorized.OnUnauthorizedAsync(ctx);
                    }
                }
                else
                {
                    await unauthorized.OnUnauthorizedAsync(ctx);
                }
            }
            catch (CommandParsingException e)
            {
                _logger.LogDebug($"An error occurred while parsing the input '{request.Input}'.", e);
                ctx?.WriteError(e);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while processing the request with the input '{request.Input}'.");

                if (ctx != null)
                {
                    if (_options.DisplayExceptions)
                    {
                        ctx.WriteError(e, true);
                    }
                    else
                    {
                        ctx.WriteError("An error occurred, please check your application logs for more details.");
                    }
                }
            }

            return response;
        }

        private static async Task ExecuteCommandAsync(CommandContext context)
        {
            var builder = new CommandBuilder(context);

            await context.Command.ExecuteAsync(builder, context);
        }

        /// <summary>
        /// Checks if the command execution is authorized.
        /// </summary>
        protected virtual async Task<bool> IsAuthorizedAsync(CommandContext context)
        {
            ICommandExecutionEnvironment environment = context.HttpContext.RequestServices.GetRequiredService<ICommandExecutionEnvironment>();

            bool authorized = await environment.IsAuthorizedAsync(context);

            if (!authorized)
            {
                CommandInfo info = context.Command.GetType().GetCommandInfo();

                _logger.LogInformation($"Command '{info.Name}' execution is unauthorized.");
            }

            return authorized;
        }
    }
}
