using System.Collections.Generic;

namespace BeavisCli.Internal
{
    internal static class WebCliRequestExtensions
    {
        /// <summary>
        /// Parses the application name from the request.
        /// </summary>
        public static string GetApplicationName(this WebCliRequest request)
        {
            if (request.Input != null)
            {
                string value = request.Input.Trim();
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
        /// Parses application arguments from the request.
        /// </summary>
        public static string[] GetApplicationArgs(this WebCliRequest request)
        {
            List<string> args = new List<string>();        
            if (request.Input != null)
            {
                string[] tokens = request.Input.Trim().Split(' ');
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
