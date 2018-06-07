using System;
using System.Threading.Tasks;
using Beavis.Isolation.Contracts;
using JKang.IpcServiceFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Beavis.Isolation
{
    public class IsolatedModuleClient
    {
        public async Task<ResponseEnvolope> HandleRequest(IsolatedModuleHandle handle, HttpContext context)
        {
            var request = new ModuleRequest(context);

            // TODO: tämä lähtee pois
            request.Data = "moi";

            var response = await HandleRequestAsync(handle, request);
            if (!response.Succeed)
            {
                if ((await Ping(handle)).Succeed)
                {
                    response = await HandleRequestAsync(handle, request);
                }
            }
            return response;
        }

        public async Task<ResponseEnvolope> Ping(IsolatedModuleHandle handle)
        {
            ResponseEnvolope envelope;

            try
            {
                var client = new IpcServiceClient<IIsolatedModule>(handle.PipeName);
                var response = await client.InvokeAsync(x => x.Ping(ModuleRequest.Empty()));
                envelope = new ResponseEnvolope(response);
            }
            catch (Exception exception)
            {
                envelope = new ResponseEnvolope(exception);
            }

            return envelope;
        }

        private async Task<ResponseEnvolope> HandleRequestAsync(IsolatedModuleHandle handle, ModuleRequest request)
        {
            ResponseEnvolope envelope;

            try
            {
                var client = new IpcServiceClient<IIsolatedModule>(handle.PipeName);
                var response = await client.InvokeAsync(x => x.HandleRequest(request));
                envelope = new ResponseEnvolope(response);
            }
            catch (Exception exception)
            {
                envelope = new ResponseEnvolope(exception);
            }

            return envelope;
        }

    }
}
