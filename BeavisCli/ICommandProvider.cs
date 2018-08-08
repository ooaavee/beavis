using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BeavisCli
{
    public interface ICommandProvider
    {
        ICommand GetCommand(string name, HttpContext httpContext);

        IEnumerable<ICommand> GetCommands(HttpContext httpContext);
    }
}
