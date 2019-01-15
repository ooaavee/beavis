using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BeavisCli
{
    public class CommandContext
    {
        public CommandContext(HttpContext httpContext, Request request, Response response, CommandInfo info, ICommand command)
        {
            HttpContext = httpContext;
            Request = request;
            Response = response;
            Info = info;
            Command = command;

            // these TextWriters are for writing console out and error messages, just
            // like Console.Out and Console.Error           
            OutWriter = new ResponseMessageTextWriter(WriteOut);
            ErrorWriter = new ResponseMessageTextWriter(WriteError);

            Processor = new CommandLineApplication
            {
                Name = info.Name,
                FullName = info.FullName,
                Description = info.Description,
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
        public HttpContext HttpContext { get; set; }

        /// <summary>
        /// Request
        /// </summary>
        public Request Request { get; set; }

        /// <summary>
        /// Response
        /// </summary>
        public Response Response { get; set; }

        /// <summary>
        /// Text writer for out message, like Console.Out
        /// </summary>
        public TextWriter OutWriter { get; set; }

        /// <summary>
        /// Text writer for error messages, like Console.Error
        /// </summary>
        public TextWriter ErrorWriter { get; set; }

        /// <summary>
        /// Information about the current command
        /// </summary>
        public CommandInfo Info { get; set; }

        /// <summary>
        /// Current command
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// ITerminalInitializer service for the current command
        /// </summary>
        public virtual ITerminalInitializer TerminalInitializer
        {
            get
            {
                if (HttpContext == null)
                {
                    throw new InvalidOperationException("HTTP Context is not available.");
                }

                return HttpContext.RequestServices.GetRequiredService<ITerminalInitializer>();
            }
        } 

        /// <summary>
        /// IFileStorage service for the current command
        /// </summary>
        public virtual IFileStorage FileStorage
        {
            get
            {
                if (HttpContext == null)
                {
                    throw new InvalidOperationException("HTTP Context is not available.");
                }

                return HttpContext.RequestServices.GetRequiredService<IFileStorage>();
            }
        }

        public void OnRequestChanged(HttpContext newHttpContext, Response newResponse)
        {
            HttpContext = newHttpContext;
            Response = newResponse;
        }

        private void WriteOut(string text)
        {
            Response.Messages.Add(ResponseMessage.Plain(text));
        }

        private void WriteError(string text)
        {
            Response.Messages.Add(ResponseMessage.Error(text));
        }

        private Task OnResponseSent()
        {
            // clear response messages
            Response.Clear();

            // forget the HTTP context
            HttpContext = null;

            return Task.CompletedTask;
        }
    }
}
