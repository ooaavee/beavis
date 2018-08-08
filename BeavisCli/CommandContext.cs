using BeavisCli.Microsoft.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;

namespace BeavisCli
{
    public class CommandContext
    {
        public Dictionary<string, object> Items { get; } = new Dictionary<string, object>();

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
        /// Writer for out message, like Console.Out
        /// </summary>
        public virtual TextWriter OutWriter { get; set; }

        /// <summary>
        /// Writer for error messages, like Console.Error
        /// </summary>
        public virtual TextWriter ErrorWriter { get; set; }

        public virtual CommandInfo Info { get; set; }

        public virtual ICommand Command { get; set; }


        /*
         *
         *
         * TODO:
         * Tänne olioviittauksena ainakin IFileStorage ja ITerminalInitializer
         *
         *
         *
         */

    }
}
