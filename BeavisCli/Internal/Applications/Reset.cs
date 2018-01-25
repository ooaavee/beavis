using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BeavisCli.JavaScriptStatements;

namespace BeavisCli.Internal.Applications
{
    internal class Reset : WebCliApplication
    {
        public Reset() : base("reset", "Resets the terminal.") { }


        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
              
                //context.Response.AddStatement(new ClearTerminal());
                //context.Response.AddStatement(new ClearTerminalHistory());


                var path = @"C:\Projects\beavis-cli\WebSite\wwwroot\images\beavis.png";

                var data = File.ReadAllBytes(path);


                 context.Response.WriteFile(data, "beavis.png", "image/png");

                //string js = $"$ctrl.upload(terminal);";

                //context.Response.Statements.Add(js);



                return Exit(context);
            }, context);

        }
    }
}
