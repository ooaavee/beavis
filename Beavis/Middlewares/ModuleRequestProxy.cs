using Beavis.Isolation;
using Beavis.Isolation.Contracts;
using Beavis.Modules;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Beavis.Http;

namespace Beavis.Middlewares
{
    public class ModuleRequestProxy
    {
        private readonly RequestDelegate _next;

        private readonly ModuleManager _modules;
        private readonly IsolationManager _isolation;
        private readonly IsolatedModuleClient _client;

        public ModuleRequestProxy(RequestDelegate next, ModuleManager modules, IsolationManager isolation, IsolatedModuleClient client)
        {
            _next = next;
            _modules = modules;
            _isolation = isolation;
            _client = client;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = ModuleHttpContext.GetMainPath(context);

            if (path == null)
            {
                await _next(context);
                return;
            }

            ModuleInfo module = _modules.GetModuleByPath(path);

            if (module == null)
            {
                await _next(context);
                return;
            }


            IsolatedModuleHandle handle = _isolation.GetIsolatedModuleHandle(module);

            if (handle == null)
            {
                await _next(context);
                return;
            }

            ResponseEnvolope envolope = await _client.HandleRequest(handle, context);

            if (envolope.Succeed)
            {
                ModuleHttpContext.WriteResponse(envolope.Content.Content, context);
                // TODO: Kirjoita Http response sillä datalla, joka on vastauksessa
            }
            else
            {
                // TODO: Kirjoitaa Internal Server Error jollakin custom viestillä, josta pystytään päättelemään, että meni vituiksi tässä päässä
            }



            await _next(context);
        }
    }
}
