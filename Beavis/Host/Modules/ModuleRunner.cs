using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Beavis.Configuration;
using Beavis.Isolation.Contracts;

namespace Beavis.Host.Modules
{
    public class ModuleRunner
    {
        private const string FileName = "Beavis.dll";

        private readonly ConfigurationAccessor _configuration;

        public ModuleRunner(ConfigurationAccessor configuration)
        {
            _configuration = configuration;
        }

        public void Run(ModuleHandle handle)
        {
            var options = GetStartupOptions(handle);

            NewProcess(options);
        }

        private ModuleStartupOptions GetStartupOptions(ModuleHandle handle)
        {
            var options = new ModuleStartupOptions
            {
                ModuleKey = handle.Module.Key,
                PipeName = handle.PipeName,
                Configuration = _configuration.GetConfiguration()
            };
            return options;
        }

        private void NewProcess(ModuleStartupOptions options)
        {
            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"{AppContext.BaseDirectory}{FileName} {options.ToCommandLineArgs()}",
                    WorkingDirectory = AppContext.BaseDirectory,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };
          
            process.Exited += delegate (object sender, EventArgs args)
            {
                Debugger.Break();
                // TODO: logging
            };

            process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs args)
            {
                Debugger.Break();
                // TODO: logging
            };

            process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs args)
            {
                Debugger.Break();
                // TODO: logging
            };

            process.Start();
        }


    }
}
