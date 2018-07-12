using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Beavis.Configuration;

namespace Beavis.Modules
{
    public class ModuleRunner
    {
        private readonly ConfigurationAccessor _configuration;

        public ModuleRunner(ConfigurationAccessor configuration)
        {
            _configuration = configuration;
        }

        public void Run(ModuleHandle handle)
        {           
            string path = Path.Combine(handle.BaseDirectory.FullName, "Beavis.dll");

            string options = GetOptions(handle).ToCommandLineArgs();

            string args = $"{path} {options}";

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = args,
                    WorkingDirectory = handle.BaseDirectory.FullName,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };

            process.Exited += delegate (object sender, EventArgs e)
            {
                Debugger.Break();
                // TODO: logging
            };

            process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Debugger.Break();
                // TODO: logging
            };

            process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Debugger.Break();
                // TODO: logging
            };

            process.Start();

            Thread.Sleep(1000);

        }

        private ModuleRuntimeOptions GetOptions(ModuleHandle handle)
        {
            var options = new ModuleRuntimeOptions
            {
                ModuleKey = handle.Module.Key,
                PipeName = handle.PipeName,
                BaseDirectory = handle.BaseDirectory.ToString(),
                AssemblyFileName = "TODO: set module assembly.dll here",
                Configuration = _configuration.GetConfiguration()
            };
            return options;
        }
    }
}
