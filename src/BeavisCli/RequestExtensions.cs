using System.Collections.Generic;

namespace BeavisCli
{
    public static class RequestExtensions
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public static string GetCommandName(this Request request)
        {
            if (request.Input != null)
            {
                var value = request.Input.Trim();
                var tokens = value.Split(' ');
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
        public static string[] GetCommandArgs(this Request request)
        {
            var args = new List<string>();
            if (request.Input != null)
            {
                var tokens = request.Input.Trim().Split(' ');
                if (tokens.Length > 1)
                {
                    for (var i = 1; i <= tokens.Length - 1; i++)
                    {
                        var arg = tokens[i].Trim();
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
