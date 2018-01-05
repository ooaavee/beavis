using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BeavisCli
{
    public class TerminalRequest
    {
        [JsonProperty("input")]
        public string Input { get; set; }

        /// <summary>
        /// Gets the application name from the request.
        /// </summary>
        public string GetApplicationName()
        {
            var value = Input.Trim();
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
        public string[] GetApplicationArgs()
        {
            var args = new List<string>();
            var input = Input.Trim();
            var all = input.Split(' ');

            if (all.Length > 1)
            {
                for (var i = 1; i <= all.Length - 1; i++)
                {
                    var arg = all[i];
                    arg = arg.Trim();

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
