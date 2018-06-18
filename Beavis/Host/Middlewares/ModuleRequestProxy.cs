using System.Diagnostics;
using Beavis.Isolation;
using Beavis.Isolation.Contracts;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Beavis.Host.Modules;
using Beavis.Http;

namespace Beavis.Middlewares
{
    public class ModuleRequestProxy
    {
        private readonly RequestDelegate _next;

        private readonly ModuleManager _moduleManager;

        public ModuleRequestProxy(RequestDelegate next, ModuleManager moduleManager)
        {
            _next = next;
            _moduleManager = moduleManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = ModuleHttpContext.GetMainPath(context);
            if (path == null)
            {
                await _next(context);
                return;
            }

            ModuleInfo module = _moduleManager.GetModuleByPath(path);
            if (module == null)
            {
                await _next(context);
                return;
            }


            ModuleHandle handle = _moduleManager.GetHandle(module);
            if (handle == null)
            {
                await _next(context);
                return;
            }

            ModuleClient client = _moduleManager.GetClient(handle);

            ResponseEnvolope envolope = await client.HandleRequest(context);

            if (envolope.Succeed)
            {
                await ModuleHttpContext.WriteResponseAsync(envolope.Content.Data, context);
            }
            else
            {
                await ModuleHttpContext.WriteResponseAsync(envolope.Exception.ToString(), context);
                // TODO: Kirjoitaa Internal Server Error jollakin custom viestillä, josta pystytään päättelemään, että meni vituiksi tässä päässä
            }



//            await _next(context);
        }
    }
}
