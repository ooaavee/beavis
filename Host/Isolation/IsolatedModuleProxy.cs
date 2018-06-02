using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Beavis.Shared;
using JKang.IpcServiceFramework;

namespace Beavis.Host.Isolation
{
    public class IsolatedModuleProxy
    {
        public async Task<ModuleResponse> XxxxAsync(IsolatedModuleHandle handle, ModuleRequest request)
        {
            var client = new IpcServiceClient<IModuleRequestHandler>(handle.PipeName);

            ModuleResponse response = await client.InvokeAsync(x => x.Handle(request));

            return response;
        }
    }
}
