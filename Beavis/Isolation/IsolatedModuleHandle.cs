using Beavis.Modules;

namespace Beavis.Isolation
{
    public class IsolatedModuleHandle
    {
        /// <summary>
        /// Module
        /// </summary>
        public ModuleInfo Module { get; set; }

        /// <summary>
        /// A named pipe name
        /// </summary>
        public string PipeName { get; set; }
    }
}
