using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Beavis.Ipc;

namespace Beavis.Modules
{
    public class ModuleManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ModuleDeployer _deployer;
        private readonly ModuleRunner _runner;

        private readonly ConcurrentDictionary<string, ModuleHandle> _handles = new ConcurrentDictionary<string, ModuleHandle>();
        private readonly ConcurrentDictionary<string, BeavisClient> _clients = new ConcurrentDictionary<string, BeavisClient>();



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
                Key = path.Substring(1),
                Path = path
            };
            return _modules[path];
        }

        public BeavisClient GetClient(ModuleInfo module)
        {
            if (_clients.TryGetValue(module.Key, out var client))
            {
                return client;
            }

            ModuleHandle handle = GetHandle(module);

            client = BeavisClient.Create(handle);

            _clients.TryAdd(module.Key, client);
            return client;
        }

        private ModuleHandle GetHandle(ModuleInfo module)
        {
            if (_handles.TryGetValue(module.Key, out var handle))
            {
                return handle;
            }

            handle = Deploy(module);
            Run(handle);

            _handles.TryAdd(module.Key, handle);
            return handle;
        }

        private ModuleHandle Deploy(ModuleInfo module)
        {
            ModuleHandle handle = _deployer.Deploy(module);
            return handle;
        }

        private void Run(ModuleHandle handle)
        {
            _runner.Run(handle);
        }

    }
}
