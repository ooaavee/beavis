using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
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

            // response
            Response response = new Response(httpContext);

            CommandContext context = null;

            try
            {
                _logger.LogDebug($"Started to process a request '{request.Input}'.");
            
                // command name entered by the user
                string name = request.GetCommandName();

                _logger.LogDebug($"Searching an command by the name '{name}'.");

                // find the command by using the ICommandProvider service
                ICommand cmd;
                try
                {
                    cmd = commands.GetCommand(name, httpContext);
                }
                catch (Exception e)
                {
                    _logger.LogDebug($"An error occurred while searching a command by using the input '{request.Input}'.", e);
                    response.Messages.Add(new ErrorMessage(e.Message));
                    return response;
                }

                _logger.LogDebug($"Found a command '{cmd.GetType().FullName}' that matches the name '{name}'.");

                CommandInfo info = CommandInfo.Get(cmd);

                // these TextWriters are for writing console out and error messages, just
                // like Console.Out and Console.Error
                TextWriter outWriter = new ResponseMessageTextWriter(delegate(string text)
                {
                    response.Messages.Add(new PlainMessage(text));
                });
                TextWriter errorWriter = new ResponseMessageTextWriter(delegate(string text)
                {
                    response.Messages.Add(new ErrorMessage(text));
                });

                CommandLineApplication processor = new CommandLineApplication
                {
                    Name = info.Name,
                    FullName = info.FullName,
                    Description = info.Description,
                    Out = outWriter,
                    Error = errorWriter
                };

                processor.HelpOption("-?|-h|--help");

                context = new CommandContext
                {
                    Processor = processor,
                    OutWriter = processor.Out,
                    ErrorWriter = processor.Error,
                    HttpContext = httpContext,
                    Request = request,
                    Response = response,
                    Info = info,
                    Command = cmd
                };

                // check authorization
                bool authorized = IsAuthorized(context);

                if (authorized)
                {
                    // execute command
                    await cmd.ExecuteAsync(context);
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

            bool isExternal = environment.GetType() != typeof(CommandExecutionEnvironment);

            if (isExternal)
            {
                _logger.LogInformation($"The authorization status returned by the current {nameof(ICommandExecutionEnvironment)} implementation '{environment.GetType().FullName}' is {authorized}.");
            }

            if (authorized)
            {
                if (!context.IsBuiltInCommand())
                {
                    _logger.LogInformation($"The authorization status returned by the command '{context.Command.GetType().FullName}' is {authorized}.");
                }
            }

            if (!context.IsBuiltInCommand() || isExternal)
            {
                _logger.LogInformation(authorized
                    ? $"The command '{context.Command.GetType().FullName}' execution is authorized."
                    : $"The command '{context.Command.GetType().FullName}' execution is unauthorized.");
            }

            return authorized;
        }
    }
}
