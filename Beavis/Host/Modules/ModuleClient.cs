using System;
using System.Threading.Tasks;
using Beavis.Ipc;
using Beavis.Isolation.Contracts;
using Microsoft.AspNetCore.Http;


namespace Beavis.Host.Modules
{
    public class ModuleClient
    {
        private readonly ModuleHandle _handle;


        public ModuleClient(ModuleHandle handle)
        {
            _handle = handle;
        }

        public async Task<ResponseEnvolope> HandleRequest(HttpContext context)
        {
            var request = new ModuleRequest(context);

            // TODO: tämä lähtee pois
            request.Data = "moi";

            ResponseEnvolope envelope;
            try
            {
                using (var client = new NamedPipeClient(_handle.PipeName))
                {
                    string response = await client.SendRequestAsync("Hello");
                    envelope = new ResponseEnvolope(new ModuleResponse() { Data = response });
                }
            }
            catch (Exception ex)
            {
                envelope = new ResponseEnvolope(ex);    
            }

            return envelope;


            //var response = await HandleRequestAsync(request);
            //if (!response.Succeed)
            //{
            //    if ((await Ping()).Succeed)
            //    {
            //        response = await HandleRequestAsync(request);
            //    }
            //}
            //return response;
        }

     

       

        //private async Task<ResponseEnvolope> HandleRequestAsync(ModuleRequest request)
        //{
        //    ResponseEnvolope envelope;

        //    try
        //    {
             
        //        var response = await Connect().InvokeAsync(x => x.HandleRequest(request));
        //        envelope = new ResponseEnvolope(response);
        //    }
        //    catch (Exception exception)
        //    {
        //        envelope = new ResponseEnvolope(exception);
        //    }

        //    return envelope;
        //}

        //private IpcServiceClient<IIsolatedModule> Connect()
        //{
        //    var client = new IpcServiceClient<IIsolatedModule>(_handle.PipeName);
        //    return client;

        //}
    }
}
