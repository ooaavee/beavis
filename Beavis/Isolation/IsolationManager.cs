using Beavis.Isolation.Contracts;
using Beavis.Modules;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Beavis.Configuration;
using Microsoft.Extensions.Configuration;

namespace Beavis.Isolation
{
    public class IsolationManager
    {
        private const string FileName = "Beavis.dll";

        private readonly ConcurrentDictionary<string, IsolatedModuleHandle> _handles = new ConcurrentDictionary<string, IsolatedModuleHandle>();

        private readonly ConcurrentDictionary<string, IsolatedModuleClient> _clients = new ConcurrentDictionary<string, IsolatedModuleClient>();


        private readonly IServiceProvider _serviceProvider;
        private readonly ConfigurationAccessor _configurationAccessor;

        public IsolationManager(IServiceProvider serviceProvider, ConfigurationAccessor configurationAccessor)
        {
            _serviceProvider = serviceProvider;
            _configurationAccessor = configurationAccessor;
        }

        public IsolatedModuleHandle GetIsolatedModuleHandle(ModuleInfo module)
        {
            if (!_handles.TryGetValue(module.Key, out IsolatedModuleHandle handle))
            {
                handle = CreateHandle(module);
                ModuleRuntimeContract contract = CreateContract(handle);
                StartModule(contract);
                _handles.TryAdd(module.Key, handle);
            }
            return handle;
        }


        public IsolatedModuleClient GetClient(IsolatedModuleHandle handle)
        {
            if (!_clients.TryGetValue(handle.Module.Key, out IsolatedModuleClient client))
            {
                client = new IsolatedModuleClient(handle);
                _clients.TryAdd(handle.Module.Key, client);
            }
            return client;
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

        private ModuleRuntimeContract CreateContract(IsolatedModuleHandle handle)
        {
            var contract = new ModuleRuntimeContract
            {
                ModuleKey = handle.Module.Key,
                PipeName = handle.PipeName,
                ThreadCount = 4,
                Configuration = _configurationAccessor.GetData()
            };
            return contract;
        }

        private void StartModule(ModuleRuntimeContract contract)
        {                  
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
