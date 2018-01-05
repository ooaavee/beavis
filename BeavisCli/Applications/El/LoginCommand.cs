////using System.Threading.Tasks;
////using BeavisCli;
////using BeavisCli.Microsoft.Extensions.CommandLineUtils;

////namespace Jemma.Terminal.Applications.El
////{
////    internal class LoginCommand : AbstractApplicationCommand
////    {
////        public static void Register(CommandLineApplication app, TerminalExecutionContext context)
////        {
////            app.Command("login", "Authenticates and logs in to the server.", config =>
////            {
////                CommandArgument username = config.Argument("[username]", "Username");
////                CommandArgument password = config.Argument("[password]", "Password");

////                config.HelpOption("-?|-h|--help");

////                config.OnExecute(() =>
////                {
////                    if (IsMissing(username) || IsMissing(password))
////                    {
////                        config.ShowHelp("login");
////                        return 2;
////                    }

////                    var command = new LoginCommand();

////                    context.Response.WriteInformation("Login succeed!!!!");

////                    return command.Execute().Result;
////                });
////            });

////            //        cll.Command("upload", config => {
////            //            var aSource = config.Argument("[source]", "Source file");
////            //            var aPath = config.Argument("[path]", "Server path");
////            //            var mimeType = config.Option("--mimetype", "Mimetype for file extension", CommandOptionType.SingleValue);
////            //            var configServer = config.Option("--server", "Server url address (without api/...)", CommandOptionType.SingleValue);

////            //            config.HelpOption("-?|-h|--help");
////            //            config.OnExecute(() => {
////            //                if (aSource.Value == null || aPath.Value == null)
////            //                {
////            //                    config.ShowHelp();
////            //                    return 2;
////            //                }
////            //                var cc = new Upload(aSource.Value, aPath.Value, configServer.HasValue() ? configServer.Value() : "http://localhost:5010");
////            //                if (mimeType.HasValue())
////            //                {
////            //                    cc.MimeType = mimeType.Value();
////            //                }
////            //                return cc.Execute().Result;
////            //            });
////            //        });


////        }

////        public LoginCommand()
////        {
            
////        }

////        private async Task<int> Execute()
////        {
////            return 0;
////        }
////    }
////}


//////public class Upload
//////{
//////    private string server;
//////    private string path;
//////    private string source;

//////    public Upload(string source, string path, string server)
//////    {
//////        this.source = source;
//////        this.path = path;
//////        this.server = server;
//////        ParseMimeType(Path.GetExtension(source).ToLower().Substring(1));
//////    }

//////    private void ParseMimeType(string ext)
//////    {
//////        if (ext == "png")
//////        {
//////            MimeType = "image/png";
//////            return;
//////        }
//////        if (ext == "jpg" || ext == "jpeg")
//////        {
//////            MimeType = "image/jpeg";
//////            return;
//////        }
//////        if (ext == "json" || ext == "babylon")
//////        {
//////            MimeType = "application/json";
//////            return;
//////        }
//////        if (ext == "html" || ext == "htm")
//////        {
//////            MimeType = "text/html";
//////            return;
//////        }
//////    }

//////    public string MimeType { get; private set; }

//////    public static void AddUpload(CommandLineApplication cll)
//////    {
//////        cll.Command("upload", config => {
//////            var aSource = config.Argument("[source]", "Source file");
//////            var aPath = config.Argument("[path]", "Server path");
//////            var mimeType = config.Option("--mimetype", "Mimetype for file extension", CommandOptionType.SingleValue);
//////            var configServer = config.Option("--server", "Server url address (without api/...)", CommandOptionType.SingleValue);

//////            config.HelpOption("-?|-h|--help");
//////            config.OnExecute(() => {
//////                if (aSource.Value == null || aPath.Value == null)
//////                {
//////                    config.ShowHelp();
//////                    return 2;
//////                }
//////                var cc = new Upload(aSource.Value, aPath.Value, configServer.HasValue() ? configServer.Value() : "http://localhost:5010");
//////                if (mimeType.HasValue())
//////                {
//////                    cc.MimeType = mimeType.Value();
//////                }
//////                return cc.Execute().Result;
//////            });
//////        });
//////    }

//////    private async Task<int> Execute()
//////    {
//////        if (MimeType == null)
//////        {
//////            Console.WriteLine("Give mime type for file {0}", source);
//////            return 4;
//////        }
//////        HttpClient cl = new HttpClient();
//////        ByteArrayContent bca = new ByteArrayContent(File.ReadAllBytes(source));
//////        string url = this.server + "/api/upload/uploadPublic?path=" + WebUtility.UrlEncode(path) + "&mimeType=" + WebUtility.UrlEncode(MimeType);
//////        // string url = this.server + "/api/upload/uploadPublic?path=abc&mimetype=png";
//////        var reply = await cl.PostAsync(url, bca);
//////        if (reply.StatusCode != HttpStatusCode.OK && reply.StatusCode != HttpStatusCode.NoContent)
//////        {
//////            Console.WriteLine("Failed to complete request, reason {0}", reply.StatusCode);
//////            var content = await reply.Content.ReadAsStringAsync();
//////            Console.WriteLine("  reply body is", content);
//////            return 4;
//////        }
//////        if (reply.StatusCode == HttpStatusCode.OK)
//////        {
//////            Console.WriteLine("Uploaded {0}", path);
//////        }
//////        else
//////        {
//////            Console.WriteLine("Unchanged {0}", path);
//////        }
//////        return 0;
//////    }
//////}
