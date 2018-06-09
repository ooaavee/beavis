using System.Diagnostics;
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

        public ModuleRequestProxy(RequestDelegate next, ModuleManager modules, IsolationManager isolation)
        {
            _next = next;
            _modules = modules;
            _isolation = isolation;
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

            IsolatedModuleClient client = _isolation.GetClient(handle);

            ResponseEnvolope envolope = await client.HandleRequest(context);

            if (envolope.Succeed)
            {
                await ModuleHttpContext.WriteResponseAsync(envolope.Content.Content, context);
                return;
            }
            else
            {
                Debugger.Break();
                // TODO: Kirjoitaa Internal Server Error jollakin custom viestillä, josta pystytään päättelemään, että meni vituiksi tässä päässä
            }



            await _next(context);
        }
    }
}
