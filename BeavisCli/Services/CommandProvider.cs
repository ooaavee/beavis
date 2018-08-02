using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace BeavisCli.Services
{
    public class CommandProvider : ICommandProvider
    {
        public virtual Command GetCommand(string name, HttpContext httpContext)
        {            
            int count = 0;

            Command result = null;

            foreach (Command cmd in GetCommands(httpContext))
            {
                if (cmd.Info.Name == name)
                {
                    count++;
                    result = cmd;
                }

                if (count > 1)
                {                    
                    throw new InvalidOperationException($"Found more than one command with name a '{name}'. Command names must me unique.");
                }
            }

            if (count == 0)
            {
                // TODO: Täällä voisi hifistellä sen verran, että jos syystä tai toisesta 'help' ei ole käytössä, niin ei myöskään kehoitetan käyttämään sitä :)

                throw new InvalidOperationException($"{name} is not a valid command.{Environment.NewLine}Usage 'help' to get list of commands.");
            }

            return result;
        }

        public virtual IEnumerable<Command> GetCommands(HttpContext httpContext)
        {            
            foreach (Command cmd in httpContext.RequestServices.GetServices<Command>())
            {
                if (cmd.Info != null)
                {
                    yield return cmd;
                }
            }
        }
    }
}
