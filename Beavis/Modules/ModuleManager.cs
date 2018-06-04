using System.Collections.Generic;

namespace Beavis.Modules
{
    public class ModuleManager
    {
        private Dictionary<string, ModuleInfo> _modules =  new Dictionary<string, ModuleInfo>();


        public ModuleInfo GetModuleByPath(string path)
        {
            if (_modules.ContainsKey(path))
            {
                return _modules[path];
            }

            _modules[path] = new ModuleInfo() { Key = path };
            return _modules[path];
        }

        //public ModuleInfo GetModuleByKey(string key)
        //{
        //    if (_modules.ContainsKey(key))
        //    {
        //        return _modules[key];
        //    }

        //    _modules[key] = new ModuleInfo() {ModuleKey = key};
        //    return _modules[key];
        //}


    }
}
