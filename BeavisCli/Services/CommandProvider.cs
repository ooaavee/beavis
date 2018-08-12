using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace BeavisCli.Services
{
    public class CommandProvider : ICommandProvider
    {
        public virtual ICommand GetCommand(string name, HttpContext httpContext)
        {            
            int count = 0;

            ICommand result = null;

            foreach (ICommand cmd in GetCommands(httpContext))
            {
                CommandInfo info = CommandInfo.Get(cmd);

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
                //
                // TODO: Täällä voisi hifistellä sen verran, että jos syystä tai toisesta 'help' ei ole käytössä, niin ei myöskään kehoitetan käyttämään sitä :)
                //

                throw new InvalidOperationException($"{name} is not a valid command.{Environment.NewLine}Usage 'help' to get list of commands.");
            }

            return result;
        }

        public virtual IEnumerable<ICommand> GetCommands(HttpContext httpContext)
        {            
            foreach (ICommand cmd in httpContext.RequestServices.GetServices<ICommand>())
            {
                CommandInfo info = CommandInfo.Get(cmd);
                if (info != null)
                {
                    yield return cmd;
                }
            }
        }
    }
}
