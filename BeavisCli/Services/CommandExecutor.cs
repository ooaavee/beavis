using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BeavisCli.Services
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly ILogger<CommandExecutor> _logger;
        private readonly BeavisCliOptions _options;

        public CommandExecutor(ILoggerFactory loggerFactory, IOptions<BeavisCliOptions> options)
        {
            _logger = loggerFactory.CreateLogger<CommandExecutor>();
            _options = options.Value;
        }

        public async Task<Response> ExecuteAsync(JObject requestBody, HttpContext httpContext)
        {
            // required services
            ICommandProvider commands = httpContext.RequestServices.GetRequiredService<ICommandProvider>();
            IUnauthorizedHandler unauthorized = httpContext.RequestServices.GetRequiredService<IUnauthorizedHandler>();

            Response response = new Response(httpContext);

            string requestBodyJson = null;

            try
            {
                requestBodyJson = requestBody.ToString();

                _logger.LogDebug($"Started to process a request '{requestBodyJson}'.");

                Request request = JsonConvert.DeserializeObject<Request>(requestBodyJson);

                // command name entered by the user
                string name = request.GetCommandName();

                _logger.LogDebug($"Searching an command by the name '{name}'.");

                // find the command by using the ICommandProvider service
                Command cmd;
                try
                {
                    cmd = commands.GetCommand(name, httpContext);
                }
                catch (Exception e)
                {
                    _logger.LogDebug($"An error occurred while searching a command by using the input '{request.Input}'.", e);
                    response.WriteError(e);
                    return response;
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
                    await unauthorized.OnUnauthorizedAsync(cmd, context);
                }
            }
            catch (CommandParsingException e)
            {
                _logger.LogDebug($"An error occurred while parsing the input '{requestBodyJson}'.", e);
                response.WriteError(e);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while processing the request with the input '{requestBodyJson}'.", e);

                if (_options.DisplayExceptions)
                {
                    response.WriteError(e, true);
                }
                else
                {
                    response.WriteError("An error occurred, please check your application logs for more details.");
                }
            }

            return response;
        }

        /// <summary>
        /// Checks if the command execution is authorized.
        /// </summary>
        protected virtual bool IsAuthorized(Command cmd, CommandContext context)
        {
            ICommandExecutionEnvironment environment = context.HttpContext.RequestServices.GetRequiredService<ICommandExecutionEnvironment>();

            bool authorized = environment.IsAuthorized(cmd, context);

            bool isExternal = environment.GetType() != typeof(CommandExecutionEnvironment);

            if (isExternal)
            {
                _logger.LogInformation($"The authorization status returned by the current {nameof(ICommandExecutionEnvironment)} implementation '{environment.GetType().FullName}' is {authorized}.");
            }

            if (authorized)
            {
                authorized = cmd.IsAuthorized(context);

                if (!cmd.IsBuiltIn)
                {
                    _logger.LogInformation($"The authorization status returned by the command '{cmd.GetType().FullName}' is {authorized}.");
                }
            }

            if (!cmd.IsBuiltIn || isExternal)
            {
                _logger.LogInformation(authorized
                    ? $"The command '{cmd.GetType().FullName}' execution is authorized."
                    : $"The command '{cmd.GetType().FullName}' execution is unauthorized.");
            }

            return authorized;
        }
    }
}
