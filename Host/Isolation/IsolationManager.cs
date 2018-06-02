using Beavis.Host.Modules;
using Beavis.Shared;
using System;
using System.Diagnostics;

namespace Beavis.Host.Isolation
{
    public class IsolationManager
    {
        private const string FileName = "Beavis.Entry.dll";

        private readonly IServiceProvider _serviceProvider;

        public IsolationManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private bool started = false;

        public IsolatedModuleHandle GetIsolatedModuleHandle(ModuleInfo module)
        {

            string pipeName = "myPipeName";


            if (!started)
            {
                try
                {
                    StartupContract contract = new StartupContract();
                    contract.Type = StartupTypes.Module;
                    contract.ModulePipeName = pipeName;
                    contract.ModuleThreadCount = 4;

                    Start(contract);
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }

                started = true;
            }

            return new IsolatedModuleHandle() {PipeName = pipeName };
        }


        private void Start(StartupContract contract)
        {            
            var process = new Process();

            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"{AppContext.BaseDirectory}{FileName} {contract.ToCommandLineArgs()}";
            process.StartInfo.WorkingDirectory = AppContext.BaseDirectory;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.EnableRaisingEvents = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Exited += delegate (object sender, EventArgs args)
            {
                // TODO: logging
            };

            process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs args)
            {
                // TODO: logging
            };

            process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs args)
            {
                // TODO: logging
            };


            process.Start();


        }


    }
}
