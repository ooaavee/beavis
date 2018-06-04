using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Beavis.Isolation.Contracts
{
    public class ModuleStartupContract
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
        /// Number of threads for this module.
        /// </summary>
        public int ThreadCount { get; set; }


        public static bool TryParse(string[] args, out ModuleStartupContract contract)
        {
            contract = Decode(args);
            return contract != null;
        }

        public string ToCommandLineArgs()
        {
            return Encode(this);
        }

        private static ModuleStartupContract Decode(string[] args)
        {
            try
            {
                var base64EncodedData = args.First();
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                var json = Encoding.UTF8.GetString(base64EncodedBytes);
                return JsonConvert.DeserializeObject<ModuleStartupContract>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string Encode(ModuleStartupContract contract)
        {
            var json = JsonConvert.SerializeObject(contract);
            var bytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }
    }
}
