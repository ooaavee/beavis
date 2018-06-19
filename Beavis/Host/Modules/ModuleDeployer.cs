using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Beavis.Configuration;

namespace Beavis.Host.Modules
{
    public class ModuleDeployer
    {
        private readonly ConfigurationAccessor _configuration;
        private readonly ModuleHostingEnvironment _hostingEnvironment;

        public ModuleDeployer(ConfigurationAccessor configuration, ModuleHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public void Deploy(ModuleInfo module)
        {
            var s = _hostingEnvironment.GetRootDirectory();


            var ffff = AppContext.BaseDirectory;

            var dir = new DirectoryInfo(AppContext.BaseDirectory);

            var files = dir.GetFiles("*", SearchOption.AllDirectories);


        }

    }
}
