using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BeavisCli
{
    public class WebCliRequest
    {
        /// <summary>
        /// An input string from the web client.
        /// </summary>
        [JsonProperty("input")]
        public string Input { get; set; }

        /// <summary>
        /// Gets the application name from the request.
        /// </summary>
        public string GetApplicationName()
        {
            string value = null;

            if (Input != null)
            {
                value = Input.Trim();

                string[] tokens = value.Split(' ');

                if (tokens.Any())
                {
                    value = tokens.First();
                }
            }

            return value;
        }

        /// <summary>
        /// Gets application arguments from the request.
        /// </summary>
        public string[] GetArgs()
        {
            var args = new List<string>();

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
