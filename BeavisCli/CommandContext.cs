using System;
using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace BeavisCli
{
    public class CommandContext
    {
        internal CommandLineApplication Processor { get; set; }

        /// <summary>
        /// HTTP context
        /// </summary>
        public virtual HttpContext HttpContext { get; set; }

        /// <summary>
        /// Request
        /// </summary>
        public virtual Request Request { get; set; }

        /// <summary>
        /// Response
        /// </summary>
        public virtual Response Response { get; set; }

        /// <summary>
        /// Text writer for out message, like Console.Out
        /// </summary>
        public virtual TextWriter OutWriter { get; set; }

        /// <summary>
        /// Text writer for error messages, like Console.Error
        /// </summary>
        public virtual TextWriter ErrorWriter { get; set; }

        public virtual CommandInfo Info { get; set; }

        public virtual ICommand Command { get; set; }

        public virtual ITerminalInitializer TerminalInitializer
        {
            get
            {
                if (HttpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available.");
                }
                return HttpContext.RequestServices.GetRequiredService<ITerminalInitializer>();
            }
        }

        public virtual IFileStorage FileStorage
        {
            get
            {
                if (HttpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available.");
                }
                return HttpContext.RequestServices.GetRequiredService<IFileStorage>();
            }
        }
    }
}
