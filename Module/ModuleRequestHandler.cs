using System;
using System.Collections.Generic;
using System.Text;
using Beavis.Shared;

namespace Beavis.Module
{
    public class ModuleRequestHandler : IModuleRequestHandler
    {
        public ModuleResponse Handle(ModuleRequest request)
        {
            var response = new ModuleResponse();
            response.Data = "HUUDAN: " + request.Data + " " + DateTime.Now.ToString();
            return response;
        }
    }
}
