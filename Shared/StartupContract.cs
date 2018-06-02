using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Beavis.Shared
{
    public class StartupContract
    {
        public StartupTypes Type { get; set; }

        public string ModulePipeName { get; set; }

        public int ModuleThreadCount { get; set; }


        /// <summary>
        /// Parse startup contract from command-line arguments.
        /// </summary>
        /// <param name="args">command-line arguments</param>
        /// <returns></returns>
        public static StartupContract FromCommandLineArguments(string[] args)
        {
            StartupContract contract = Decode(args);

            if (contract == null)
            {
                contract = new StartupContract();
                contract.Type = StartupTypes.Host;
            }

            return contract;
        }

        public string ToCommandLineArgs()
        {
            return Encode(this);
        }

        private static StartupContract Decode(string[] args)
        {
            try
            {
                var base64EncodedData = args.First();
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                var json = Encoding.UTF8.GetString(base64EncodedBytes);
                return JsonConvert.DeserializeObject<StartupContract>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string Encode(StartupContract contract)
        {
            var json = JsonConvert.SerializeObject(contract);
            var bytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }
    }
}
