using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace BeavisCli
{
    public static class RequestExtensions
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public static string GetCommandName(this Request request)
        {
            if (request.Input == null)
            {
                return null;
            }

            string value = request.Input.Trim();

            string[] tokens = value.Split(' ');

            if (tokens.Length > 0)
            {
                value = tokens[0];
            }

            return value;
        }

        /// <summary>
        /// Gets command arguments.
        /// </summary>
        public static string[] GetCommandArgs(this Request request)
        {
            var args = new List<string>();

            if (request.Input == null)
            {
                return args.ToArray();
            }

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

            return args.ToArray();
        }
    }
}
