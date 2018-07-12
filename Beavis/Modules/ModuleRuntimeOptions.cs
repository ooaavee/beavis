using Beavis.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beavis.Modules
{
    public class ModuleRuntimeOptions
    {      
        /// <summary>
        /// Module key
        /// </summary>
        public string ModuleKey { get; set; }

        /// <summary>
        /// A named pipe name
        /// </summary>
        public string PipeName { get; set; }

        /// <summary>
        /// Module base directory
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// Module assembly file name, like Contoso.Plugin.dll
        /// </summary>
        public string AssemblyFileName { get; set; }

        /// <summary>
        /// Module configuration properties
        /// </summary>
        public Dictionary<string, string> Configuration { get; set; } = new Dictionary<string, string>();

        public static bool TryParse(string[] args, out ModuleRuntimeOptions options)
        {
            options = null;
            if (args != null && args.Any())
            {
                options = Decode(args);
            }
            return options != null;
        }

        public IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.Sources.Add(new DictionaryConfigurationSource(Configuration));
            return builder.Build();
        }

        public string ToCommandLineArgs()
        {
            var bytes = Encoding.UTF8.GetBytes(AsSerialized());
            return Convert.ToBase64String(bytes);
        }

        public string AsSerialized()
        {
            var json = JsonConvert.SerializeObject(this);
            return json;
        }

        private static ModuleRuntimeOptions Decode(string[] args)
        {
            try
            {
                var base64EncodedData = args.First();
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                var json = Encoding.UTF8.GetString(base64EncodedBytes);
                return JsonConvert.DeserializeObject<ModuleRuntimeOptions>(json);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
