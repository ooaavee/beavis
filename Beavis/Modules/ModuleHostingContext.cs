using System;

namespace Beavis.Modules
{
    public class ModuleHostingContext
    {
        /// <summary>
        /// Gets the current ModuleHostingContext.
        /// </summary>
        public static ModuleHostingContext Current { get; private set; }

        /// <summary>
        /// Options
        /// </summary>
        public ModuleRuntimeOptions Options { get; private set; }

        public static void Build(ModuleRuntimeOptions options)
        {
            if (Current != null)
            {
                throw new InvalidOperationException("Already built.");
            }

            Current = new ModuleHostingContext { Options = options };
        }
    }
}
