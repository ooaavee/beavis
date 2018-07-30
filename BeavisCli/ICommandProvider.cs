using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BeavisCli
{
    public interface ICommandProvider
    {
        Command GetCommand(string name, HttpContext httpContext);

        IEnumerable<Command> GetCommands(HttpContext httpContext);
    }
}
