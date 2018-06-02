using System;
using System.Collections.Generic;
using System.Text;

namespace Beavis.Shared
{
    public interface IModuleRequestHandler
    {
        ModuleResponse Handle(ModuleRequest request);
    }
}
