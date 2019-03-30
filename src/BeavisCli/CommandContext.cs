using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using BeavisCli.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BeavisCli
{
    public class CommandContext
    {
        public CommandContext(HttpContext context, Request request, Response response, CommandInfo info, ICommand command)
        {
            HttpContext = context ?? throw new ArgumentNullException(nameof(context));
            Request = request ?? throw new ArgumentNullException(nameof(request));
            Response = response ?? throw new ArgumentNullException(nameof(response));
            Info = info ?? throw new ArgumentNullException(nameof(info));
            Command = command ?? throw new ArgumentNullException(nameof(command));

            // these TextWriters are for writing console out and error messages, just
            // like Console.Out and Console.Error           
            OutWriter = new ResponseMessageTextWriter(text =>
            {
                Response.Messages.Add(ResponseMessage.Plain(text));
            });

            ErrorWriter = new ResponseMessageTextWriter(text =>
            {
                Response.Messages.Add(ResponseMessage.Error(text));
            });

            Processor = new CommandLineApplication
            {
                Name = info.Name,
                FullName = info.Description,
                Description = info.LongDescription,
                Out = OutWriter,
                Error = ErrorWriter
            };

            Processor.HelpOption("-?|-h|--help");

            HttpContext.Response.OnCompleted(OnResponseSent);
        }

        internal CommandLineApplication Processor { get; }

        /// <summary>
        /// HTTP context
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// Request
        /// </summary>
        public Request Request { get; }

        /// <summary>
        /// Response
        /// </summary>
        public Response Response { get; private set; }

        /// <summary>
        /// Text writer for out message, like Console.Out
        /// </summary>
        public TextWriter OutWriter { get; }

        /// <summary>
        /// Text writer for error messages, like Console.Error
        /// </summary>
        public TextWriter ErrorWriter { get; }

        /// <summary>
        /// Information about the current command
        /// </summary>
        public CommandInfo Info { get; }

        /// <summary>
        /// Current command
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// IFileStorage service for the current command
        /// </summary>
        public IFileStorage FileStorage
        {
            get
            {
                if (_fileStorage == null)
                {
                    _fileStorage = HttpContext.RequestServices.GetRequiredService<IFileStorage>();
                }
                return _fileStorage;
            }
        }

        private IFileStorage _fileStorage;

        public void OnRequestChanged(HttpContext newContext, Response newResponse)
        {
            HttpContext = newContext ?? throw new ArgumentNullException(nameof(newContext));
            Response = newResponse ?? throw new ArgumentNullException(nameof(newResponse));
        }
       
        private Task OnResponseSent()
        {
            // clear response messages
            Response.Clear();

            // forget the HTTP context
            HttpContext = null;

            return Task.CompletedTask;
        }

        private sealed class ResponseMessageTextWriter : TextWriter
        {
            private readonly Action<string> _writeLine;

            public ResponseMessageTextWriter(Action<string> writeLine)
            {
                _writeLine = writeLine;
            }

            public override void WriteLine()
            {
                WriteLine(string.Empty);
            }

            public override void WriteLine(string value)
            {
                _writeLine(value);
            }

            public override void Write(char value)
            {
                throw new InvalidOperationException("we should never be here");
            }

            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
