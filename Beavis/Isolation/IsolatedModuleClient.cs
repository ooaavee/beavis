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
        private readonly IsolatedModuleHandle _handle;


        public IsolatedModuleClient(IsolatedModuleHandle handle)
        {
            _handle = handle;
        }

        public async Task<ResponseEnvolope> HandleRequest(HttpContext context)
        {
            var request = new ModuleRequest(context);

            // TODO: tämä lähtee pois
            request.Data = "moi";

            var response = await HandleRequestAsync(request);
            if (!response.Succeed)
            {
                if ((await Ping()).Succeed)
                {
                    response = await HandleRequestAsync(request);
                }
            }
            return response;
        }

     

        public async Task<ResponseEnvolope> Ping()
        {
            ResponseEnvolope envelope;

            try
            {
                var response = await Connect().InvokeAsync(x => x.Ping(ModuleRequest.Empty()));
                envelope = new ResponseEnvolope(response);
            }
            catch (Exception exception)
            {
                envelope = new ResponseEnvolope(exception);
            }

            return envelope;
        }

        private async Task<ResponseEnvolope> HandleRequestAsync(ModuleRequest request)
        {
            ResponseEnvolope envelope;

            try
            {
             
                var response = await Connect().InvokeAsync(x => x.HandleRequest(request));
                envelope = new ResponseEnvolope(response);
            }
            catch (Exception exception)
            {
                envelope = new ResponseEnvolope(exception);
            }

            return envelope;
        }

        private IpcServiceClient<IIsolatedModule> Connect()
        {
            var client = new IpcServiceClient<IIsolatedModule>(_handle.PipeName);
            return client;

        }
    }
}
