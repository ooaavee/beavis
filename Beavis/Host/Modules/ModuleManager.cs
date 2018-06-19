using Beavis.Isolation.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Beavis.Configuration;
using Microsoft.Extensions.Configuration;
using Beavis.Host.Modules;


namespace Beavis.Host.Modules
{
    public class ModuleManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ModuleDeployer _deployer;
        private readonly ModuleRunner _runner;

        private readonly ConcurrentDictionary<string, ModuleHandle> _handles = new ConcurrentDictionary<string, ModuleHandle>();
        private readonly ConcurrentDictionary<string, ModuleClient> _clients = new ConcurrentDictionary<string, ModuleClient>();



        public ModuleManager(IServiceProvider serviceProvider, ModuleDeployer deployer, ModuleRunner runner)
        {
            _serviceProvider = serviceProvider;
            _deployer = deployer;
            _runner = runner;                
        }

        private Dictionary<string, ModuleInfo> _modules = new Dictionary<string, ModuleInfo>();
        public ModuleInfo GetModuleByPath(string path)
        {
            if (_modules.ContainsKey(path))
            {
                return _modules[path];
            }

            _modules[path] = new ModuleInfo()
            {
                Key = path,
                Path = path
            };
            return _modules[path];
        }

        public ModuleHandle GetHandle(ModuleInfo module)
        {
            ModuleHandle handle;

            string key = module.Key;

            if (_handles.TryGetValue(key, out handle))
            {
                return handle;
            }

            _deployer.Deploy(module);

            handle = new ModuleHandle(module);

            _runner.Run(handle);

            _handles.TryAdd(key, handle);

            return handle;
        }
       
        public ModuleClient GetClient(ModuleHandle handle)
        {
            ModuleClient client;

            string key = handle.Module.Key;

            if (_clients.TryGetValue(key, out client))
            {
                return client;
            }

            client = new ModuleClient(handle);

            _clients.TryAdd(key, client);

            return client;
        }





    }
}
