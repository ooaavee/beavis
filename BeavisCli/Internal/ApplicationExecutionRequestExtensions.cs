using System.Collections.Generic;
using System.Linq;

namespace BeavisCli.Internal
{
    internal static class ApplicationExecutionRequestExtensions
    {
        /// <summary>
        /// Gets the application name from the request.
        /// </summary>
        public static string GetApplicationName(this ApplicationExecutionRequest request)
        {
            var value = request.Input.Trim();
            var tokens = value.Split(' ');
            if (tokens.Any())
            {
                value = tokens.First();
            }
            return value;
        }

        /// <summary>
        /// Gets application arguments from the request.
        /// </summary>
        public static string[] GetArgs(this ApplicationExecutionRequest request)
        {
            var args = new List<string>();
            var input = request.Input.Trim();
            var all = input.Split(' ');

            if (all.Length > 1)
            {
                for (var i = 1; i <= all.Length - 1; i++)
                {
                    var arg = all[i].Trim();
                    if (arg.Length > 0)
                    {
                        args.Add(arg);
                    }
                }
            }

            return args.ToArray();
        }
    }
}
