//using System.Threading.Tasks;
//using BeavisCli;
//using BeavisCli.Microsoft.Extensions.CommandLineUtils;

//namespace Jemma.Terminal.Applications.Login
//{
//    internal class Login : AbstractBeavisApplication
//    {
//        public static readonly ApplicationInfo Definition = new ApplicationInfo
//        {
//            ////Type = typeof(Login),
//            Name = "login",
//            Description = "Authenticates and logs in to the server."
//        };

//        protected override async Task OnRunAsync(WebCliContext context)
//        {
//            var app = CreateApplication(Definition, context);

//            CommandArgument apiKey = app.Argument("[key]", "API key used for authentication.");

//            app.OnExecute(() =>
//            {             
//                if (!HasArgument(apiKey))
//                {
//                    app.ShowHelp(Definition.Name);
//                    return Exit();
//                }


//                using (context.Response.BeginInteraction())
//                {
//                    if (true)
//                    {
//                        context.Response.WriteSuccess("Login succeed.");
//                    }
//                    else
//                    {
//                        context.Response.WriteError("Login failed.");
//                    }
//                }

//                return Exit();
//            });

//            Execute(app, context);
//        }

//    }
//}
