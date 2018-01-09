//using System.Threading.Tasks;
//using BeavisCli;
//using BeavisCli.Microsoft.Extensions.CommandLineUtils;
//using CommandOptionType = BeavisCli.Microsoft.Extensions.CommandLineUtils.CommandOptionType;

//namespace Jemma.Terminal.Applications.El
//{
//    internal class El : AbstractBeavisApplication
//    {
//        public static readonly ApplicationInfo Definition = new ApplicationInfo
//        {
//            ////Type = typeof(El),
//            Name = "el",
//            Description = "This utility is used to manage and query data."
//        };

//        protected override async Task OnRunAsync(CliContext context)
//        {
//            CommandLineApplication app = CreateApplication(Definition, context);
//            app.FullName = "event-lake command line util";
//            app.Description = "This utility is used to manage and query data.";

//            app.HelpOption("-?|-h|--help");

//            CommandOption option1 = app.Option("-option1", "Tämä on option 1 as djalks djlaskdj salkjd sakldj aöjkasljlasldj jd j d kj lkj dlkasjd djlaskdj salkjd sakldj aöjkasljlasldj jd j d kj lkj dlkasjd djlaskdj salkjd sakldj aöjkasljlasldj jd j d kj lkj dlkasjd djlaskdj salkjd sakldj aöjkasljlasldj jd j d kj lkj dlkasjd djlaskdj salkjd sakldj aöjkasljlasldj jd j d kj lkj dlkasjd djlaskdj salkjd sakldj aöjkasljlasldj jd j d kj lkj dlkasjd djlaskdj salkjd sakldj aöjkasljlasldj jd j d kj lkj dlkasjd djlaskdj salkjd sakldj aöjkasljlasldj jd j d kj lkj dlkasjd.", CommandOptionType.SingleValue);
//            CommandOption option2 = app.Option("-option2", "Tämä on option 2", CommandOptionType.SingleValue);
            
//            ////LoginCommand.Register(app, context);
//            app.OnExecute(() =>
//            {
//                var v1 = option1.Value();


//                var v2 = option2.Value();


//                app.ShowHelp();
//                return 2;
//            });

//            //RCompile.AddCompile(cll);
//            //Upload.AddUpload(cll);
//            //cll.OnExecute(() => {
//            //    cll.ShowHelp();
//            //    return 2;
//            //});
//            //return cll.Execute(args);

//            Execute(app, context);
//        }


//        //private static void InitServices()
//        //{
//        //    ServiceCollection sc = new ServiceCollection();
//        //    ServiceProvider = sc.BuildServiceProvider();
//        //}

//    }
//}
