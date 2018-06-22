using System;
using System.IO;

namespace Beavis.Modules
{
    public class ModuleHandle
    {
        public ModuleHandle(ModuleInfo module, DirectoryInfo baseDirectory)
        {
            Module = module;
            PipeName = NewPipeName();
            BaseDirectory = baseDirectory;
        }

        /// <summary>
        /// Module
        /// </summary>
        public ModuleInfo Module { get; }

        /// <summary>
        /// A named pipe name
        /// </summary>
        public string PipeName { get; }

        /// <summary>
        /// Module base directory
        /// </summary>
        public DirectoryInfo BaseDirectory { get; }

        private static string NewPipeName()
        {
            var s = Guid.NewGuid().ToString();
            var i = s.IndexOf("-", StringComparison.Ordinal);
            var v = s.Substring(0, i);
            return v;
        }
    }
}
