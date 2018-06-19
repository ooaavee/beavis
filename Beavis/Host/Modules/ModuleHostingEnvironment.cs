using Beavis.Configuration;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Beavis.Host.Modules
{
    public class ModuleHostingEnvironment
    {
        private readonly ConfigurationAccessor _configuration;

        public ModuleHostingEnvironment(ConfigurationAccessor configuration, IHostingEnvironment env)
        {
            _configuration = configuration;

        }

        /// <summary>
        /// Gets a root directory for hosted modules.
        /// </summary>
        public DirectoryInfo GetRootDirectory()
        {
            string rootDirectory = _configuration.GetConfiguration()["MODULE_HOSTING_ROOT_DIRECTORY"];

            if (string.IsNullOrEmpty(rootDirectory))
            {
                if (IsAzureAppService())
                {
                    rootDirectory = "d:/home/beavis";
                }
                else
                {
                    rootDirectory = Path.Combine(Path.GetTempPath(), "beavis");
                }
            }

            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }

            DirectoryInfo dir = new DirectoryInfo(rootDirectory);
            return dir;
        }

        /// <summary>
        /// Checks if we are running Azure App Service.
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
