using Beavis.Isolation.Contracts;
using Beavis.Modules;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Beavis.Isolation
{
    public class IsolationManager
    {
        private const string FileName = "Beavis.dll";

        private readonly ConcurrentDictionary<string, IsolatedModuleHandle> _handles = new ConcurrentDictionary<string, IsolatedModuleHandle>();
        private readonly IServiceProvider _serviceProvider;

        public IsolationManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IsolatedModuleHandle GetIsolatedModuleHandle(ModuleInfo module)
        {
            if (!_handles.TryGetValue(module.Key, out IsolatedModuleHandle handle))
            {
                handle = CreateHandle(module);
                StartProcess(handle);
                _handles.TryAdd(module.Key, handle);
            }

            return handle;           
        }


        public IsolatedModuleClient GetClient(IsolatedModuleHandle handle)
        {
            // TODO: Muista pitää clientit jemmassa, jotta niitä voidaan uudelleenkäyttää.

            throw new NotImplementedException();
        }

        private static IsolatedModuleHandle CreateHandle(ModuleInfo module)
        {
            var handle = new IsolatedModuleHandle();
            handle.Module = module;
            handle.PipeName = NewRndKey();
            return handle;
        }

        private static string NewRndKey()
        {
            var s = Guid.NewGuid().ToString();
            var i = s.IndexOf("-", StringComparison.Ordinal);
            var v = s.Substring(0, i);
            return v;
        }

        private void StartProcess(IsolatedModuleHandle handle)
        {
            var contract = new ModuleRuntimeContract
            {
                ModuleKey = handle.Module.Key,
                PipeName = handle.PipeName,
                ThreadCount = 4
            };

            contract.Configuration["connectionString"] = "sdsad";
            contract.Configuration["connectionString2"] = "asdasd";

            var c = contract.GetConfiguration();

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"{AppContext.BaseDirectory}{FileName} {contract.ToCommandLineArgs()}",
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
