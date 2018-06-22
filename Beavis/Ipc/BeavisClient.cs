using Microsoft.AspNetCore.Http;
using System;
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

        public async Task<ModuleResponse> ProcessRequestAsync(HttpRequest request)
        {
            string requestMesssage = BeavisProtocol.CreateRequestMessage(request);
            string responseMessage;
         
            try
            {
                using (var client = new NamedPipeClient(_handle.PipeName))
                {
                    responseMessage = await client.SendRequestAsync(requestMesssage);
                }
            }
            catch (Exception e)
            {
                return new ModuleResponse(e);
            }

            return new ModuleResponse(BeavisProtocol.CreateResponseModel(responseMessage)); 
        }
    }
}
