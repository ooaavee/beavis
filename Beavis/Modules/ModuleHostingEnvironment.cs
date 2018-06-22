using System;
using System.IO;
using Beavis.Configuration;

namespace Beavis.Modules
{
    public class ModuleHostingEnvironment
    {
        private readonly ConfigurationAccessor _configuration;

        public ModuleHostingEnvironment(ConfigurationAccessor configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gets a root directory for hosted modules.
        /// </summary>
        public DirectoryInfo GetRootDirectory()
        {
            var path = _configuration["MODULE_HOSTING_ROOT_DIRECTORY"];

            if (string.IsNullOrEmpty(path))
            {
                if (IsAzureAppService())
                {
                    // this is our home directory when working with Azure App Services
                    path = "d:/home/";
                }
                else
                {
                    // otherwise we just use the temp directory
                    path = Path.GetTempPath();
                }

                path = Path.Combine(path, "beavis");
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DirectoryInfo dir = new DirectoryInfo(path);
            return dir;
        }

        public DirectoryInfo NewModuleBaseDirectory(ModuleInfo module)
        {
            var path = Path.Combine(GetRootDirectory().FullName, "modules");
            path = Path.Combine(path, module.Key);
            path = Path.Combine(path, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DirectoryInfo dir = new DirectoryInfo(path);
            return dir;
        }

        /// <summary>
        /// Checks if we are we using Azure App Services.
        /// </summary>
        private bool IsAzureAppService()
        {
            if (Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") != null &&
                Environment.GetEnvironmentVariable("WEBSITE_SKU") != null &&
                Environment.GetEnvironmentVariable("WEBSITE_COMPUTE_MODE") != null &&
                Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") != null)
            {
                return true;
            }

            return false;
        }
    }
}
