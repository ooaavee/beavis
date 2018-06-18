using System;

namespace Beavis.Host.Modules
{
    public class ModuleHandle
    {
        public ModuleHandle(ModuleInfo module)
        {
            Module = module;
            PipeName = NewPipeName();
        }

        /// <summary>
        /// Module
        /// </summary>
        public ModuleInfo Module { get; }

        /// <summary>
        /// A named pipe name
        /// </summary>
        public string PipeName { get; }

        private static string NewPipeName()
        {
            var s = Guid.NewGuid().ToString();
            var i = s.IndexOf("-", StringComparison.Ordinal);
            var v = s.Substring(0, i);
            return v;
        }
    }
}
