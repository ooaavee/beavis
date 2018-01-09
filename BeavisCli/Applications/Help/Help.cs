//using BeavisCli;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Threading.Tasks;

//namespace Jemma.Terminal.Applications.Help
//{
//    internal class Help : AbstractBeavisApplication
//    {
//        public static readonly ApplicationInfo Definition = new ApplicationInfo
//        {
//            ////Type = typeof(Help),
//            Name = "help",
//            Description = "Help"
//        };

//        protected override async Task OnRunAsync(CliContext context)
//        {
//            var app = CreateApplication(Definition, context);

//            app.OnExecute(() =>
//            {
//                Assembly assembly = GetType().GetTypeInfo().Assembly;
//                AssemblyName asmName = assembly.GetName();

//                string version = $"{asmName.Version.Major}.{asmName.Version.Minor}.{asmName.Version.Build}";
//                string title = "App Name";
//                string description = "App Description";
//                string copyright = "Copyright (c) 2018 Ossi Vartiainen";

//                using (context.Response.BeginInteraction())
//                {
//                    context.Response.WriteInformation(title + " " + version);
//                    context.Response.WriteInformation(description);
//                    context.Response.WriteInformation(copyright);
//                    context.Response.WriteEmptyLine();
//                    context.Response.WriteInformation("List of supported commands:");
//                    context.Response.WriteEmptyLine();

//                    int maxLen = 0;

//                    List<ApplicationInfo> tahanPitaaJostakinHommataKaikkiApplicaatiot = new List<ApplicationInfo>();

//                    foreach (ApplicationInfo info in tahanPitaaJostakinHommataKaikkiApplicaatiot)
//                    {
//                        // TODO: Tämä IsAvailable pitää toteuttaa tai vastaava!!!
//                        //if (ApplicationInfo.IsAvailable(info, context))
//                        {
//                            //if (info.Type != typeof(Help))
//                            {
//                                int len = info.Name.Length;
//                                if (len > maxLen)
//                                {
//                                    maxLen = len;
//                                }
//                            }
//                        }
//                    }

//                    foreach (ApplicationInfo info in tahanPitaaJostakinHommataKaikkiApplicaatiot)
//                    {
//                        // TODO: Tämä IsAvailable pitää toteuttaa tai vastaava!!!
//                        //if (ApplicationInfo.IsAvailable(info, context))
//                        {
//                            //if (info.Type != typeof(Help))
//                            {
//                                string name = (info.Name + new string(' ', maxLen)).Substring(0, maxLen + 1);
//                                context.Response.WriteInformation(string.Format("   {0}   {1}", name, info.Description));
//                            }
//                        }
//                    }
//                }

//                return Exit();
//            });

//            Execute(app, context);
//        }

//    }
//}
