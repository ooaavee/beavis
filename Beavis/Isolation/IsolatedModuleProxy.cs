using System;
using System.Threading.Tasks;
using Beavis.Isolation.Contracts;
using JKang.IpcServiceFramework;
using Microsoft.AspNetCore.Http;

namespace Beavis.Isolation
{
    public class IsolatedModuleProxy
    {      
        public async Task<ResponseEnvolope> HandleRequestAsync(IsolatedModuleHandle handle, HttpContext context)
        {
            var request = new ModuleRequest(context);

            // TODO: tämä lähtee pois
            request.Data = "moi";

            ResponseEnvolope response = await HandleRequestAsync(handle, request);
            if (!response.Succeed)
            {
                if ((await PingAsync(handle)).Succeed)
                {
                    response = await HandleRequestAsync(handle, request);
                }
            }
            return response;
        }

        public async Task<ResponseEnvolope> PingAsync(IsolatedModuleHandle handle)
        {
            try
            {
                var client = GetClient(handle);
                ModuleResponse response = await client.InvokeAsync(x => x.Ping(ModuleRequest.Empty()));

                return new ResponseEnvolope(response);
            }
            catch (Exception exception)
            {
                return new ResponseEnvolope(exception);
            }
        }

        private async Task<ResponseEnvolope> HandleRequestAsync(IsolatedModuleHandle handle, ModuleRequest request)
        {
            try
            {
                var client = GetClient(handle);
                ModuleResponse response = await client.InvokeAsync(x => x.HandleRequest(request));

                return new ResponseEnvolope(response);
            }
            catch (Exception exception)
            {
                return new ResponseEnvolope(exception);
            }
        }

        private static IpcServiceClient<IIsolatedModule> GetClient(IsolatedModuleHandle handle)
        {
            var client = new IpcServiceClient<IIsolatedModule>(handle.PipeName);
            return client;
        }

    }
}
