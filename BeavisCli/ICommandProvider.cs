using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace BeavisCli
{
    public interface ICommandProvider
    {
        WebCliCommand GetCommand(string name, HttpContext httpContext);

        IEnumerable<WebCliCommand> GetCommands(HttpContext httpContext);


    }
}
