using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using ModuleHandle = Beavis.Modules.ModuleHandle;

namespace Beavis.Ipc
{
    public class BeavisClient
    {
        private readonly ModuleHandle _handle;

        private BeavisClient(ModuleHandle handle)
        {
            _handle = handle;
        }

        public static BeavisClient Create(ModuleHandle handle)
        {
            return new BeavisClient(handle);
        }

        public async Task<ModuleResponse> ProcessRequestAsync(HttpRequest httpRequest)
        {
            string request = BeavisProtocol.CreateRequestMessage(httpRequest);
            string response;
         
            try
            {
                using (var client = new NamedPipeClient(_handle.PipeName))
                {
                    response = await client.SendRequestAsync(request);
                }
            }
            catch (Exception e)
            {
                // TODO: Logging

                return ModuleResponse.CreateFailed(e);
            }

            HttpResponseModel responseModel = BeavisProtocol.CreateResponseModel(response, out BeavisProtocolResponseStatus status);

            if (status == BeavisProtocolResponseStatus.Failed)
            {
                return ModuleResponse.CreateFailed();
            }

            return ModuleResponse.CreateSucceed(responseModel);
        }
    }
}
