using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beavis.Configuration;

namespace Beavis.Host.Modules
{
    public class ModuleDeployer
    {
        private readonly ConfigurationAccessor _configuration;

        public ModuleDeployer(ConfigurationAccessor configuration)
        {
            _configuration = configuration;
        }

        public void Deploy(ModuleInfo module)
        {

        }

    }
}
