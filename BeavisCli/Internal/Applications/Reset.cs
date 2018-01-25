using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Hosting;

namespace BeavisCli.Internal.Applications
{
    internal class Reset : WebCliApplication
    {
        private readonly IHostingEnvironment _env;

        public Reset(IHostingEnvironment env) : base("reset", "Resets the terminal.")
        {
            _env = env;
        }


        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
              
                //context.Response.AddStatement(new ClearTerminal());
                //context.Response.AddStatement(new ClearTerminalHistory());


                var path = _env.WebRootPath + "/images/beavis.png";


                var data = File.ReadAllBytes(path);


                 context.Response.WriteFile(data, "beavis.png", "image/png");

                //string js = $"$ctrl.upload(terminal);";

                //context.Response.Statements.Add(js);



                return Exit(context);
            }, context);

        }
    }
}
