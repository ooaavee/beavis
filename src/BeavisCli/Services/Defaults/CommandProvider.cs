using BeavisCli.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace BeavisCli.Services.Defaults
{
    public class CommandProvider : ICommandProvider
    {
        private readonly BeavisCliOptions _options;

        public CommandProvider(IOptions<BeavisCliOptions> options)
        {
            _options = options.Value;
        }

        public virtual ICommand Find(string name, HttpContext context)
        {            
            int count = 0;

            ICommand result = null;

            foreach (ICommand cmd in All(context))
            {
                CommandInfo info = cmd.GetType().GetCommandInfo();

                if (info.Name == name)
                {
                    count++;

                    if (count > 1)
                    {
                        throw new InvalidOperationException($"Found more than one command with name a '{name}'. Command names must me unique.");
                    }

                    result = cmd;
                }
            }

            if (count == 0)
            {
                bool help = true;

                if (_options.BuiltInCommandBehaviours.TryGetValue("help", out CommandBehaviour behaviour))
                {
                    help = behaviour.IsEnabled;
                }

                throw new InvalidOperationException(
                    help ?                     
                    $"{name} is not a valid command.{Environment.NewLine}Usage 'help' to get list of commands."
                    : 
                    $"{name} is not a valid command.");
            }

            return result;
        }

        public virtual IEnumerable<ICommand> All(HttpContext context)
        {            
            foreach (ICommand cmd in context.RequestServices.GetServices<ICommand>())
            {
                CommandInfo info = cmd.GetType().GetCommandInfo();
                if (info != null)
                {
                    yield return cmd;
                }
            }
        }
    }
}
