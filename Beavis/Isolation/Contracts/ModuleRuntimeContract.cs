using Beavis.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beavis.Isolation.Contracts
{
    public class ModuleRuntimeContract
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
        /// Number of threads for this module
        /// </summary>
        public int ThreadCount { get; set; }

        /// <summary>
        /// Module configuration properties
        /// </summary>
        public Dictionary<string, string> Configuration { get; set; } = new Dictionary<string, string>();

        public static bool TryParse(string[] args, out ModuleRuntimeContract contract)
        {
            contract = null;
            if (args != null && args.Any())
            {
                contract = Decode(args);
            }
            return contract != null;
        }

        public IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.Sources.Add(new DictionaryConfigurationSource(Configuration));
            return builder.Build();
        }

        public string ToCommandLineArgs()
        {
            return Encode(this);
        }

        private static ModuleRuntimeContract Decode(string[] args)
        {
            try
            {
                var base64EncodedData = args.First();
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                var json = Encoding.UTF8.GetString(base64EncodedBytes);
                return JsonConvert.DeserializeObject<ModuleRuntimeContract>(json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return null;
            }
        }

        private static string Encode(ModuleRuntimeContract contract)
        {
            var json = JsonConvert.SerializeObject(contract);
            var bytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }
    }
}
