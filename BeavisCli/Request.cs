using System.Collections.Generic;
using Newtonsoft.Json;

namespace BeavisCli
{
    public class Request
    {
        /// <summary>
        /// An input string from the web client.
        /// </summary>
        [JsonProperty("input")]
        public string Input { get; set; }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string GetCommandName()
        {
            if (Input != null)
            {
                string value = Input.Trim();

                string[] tokens = value.Split(' ');

                if (tokens.Length > 0)
                {
                    value = tokens[0];
                }

                return value;
            }

            return null;
        }

        /// <summary>
        /// Gets command arguments.
        /// </summary>
        public string[] GetCommandArgs()
        {
            List<string> args = new List<string>();

            if (Input != null)
            {
                string[] tokens = Input.Trim().Split(' ');

                if (tokens.Length > 1)
                {
                    for (var i = 1; i <= tokens.Length - 1; i++)
                    {
                        string arg = tokens[i].Trim();

                        if (arg.Length > 0)
                        {
                            args.Add(arg);
                        }
                    }
                }
            }

            return args.ToArray();
        }
    }
}
