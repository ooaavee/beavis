using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
            contract = Decode(args);
            return contract != null;
        }

        public IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.Sources.Add(new MyConfigurationSource(Configuration));
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
            catch (Exception)
            {
                return null;
            }
        }

        private static string Encode(ModuleRuntimeContract contract)
        {
            var json = JsonConvert.SerializeObject(contract);
            var bytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }

        public sealed class MyConfigurationSource : IConfigurationSource
        {
            private readonly IDictionary<string, string> _data;

            public MyConfigurationSource(IDictionary<string, string> data)
            {
                _data = data;
            }

            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                return new MyConfigurationProvider(_data);
            }
        }

        private sealed class MyConfigurationProvider : ConfigurationProvider
        {
            private readonly IDictionary<string, string> _data;

            public MyConfigurationProvider(IDictionary<string, string> data)
            {
                _data = data;
            }

            public override void Load()
            {
                Data = _data;
            }
        }

    }
}
