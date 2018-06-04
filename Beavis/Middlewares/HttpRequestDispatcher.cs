using Beavis.Isolation;
using Beavis.Isolation.Contracts;
using Beavis.Modules;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Beavis.Middlewares
{
    public class HttpRequestDispatcher
    {
        private readonly RequestDelegate _next;

        private readonly ModuleManager _modules;
        private readonly IsolationManager _isolation;
        private readonly IsolatedModuleProxy _proxy;

        public HttpRequestDispatcher(RequestDelegate next, ModuleManager modules, IsolationManager isolation, IsolatedModuleProxy proxy)
        {
            _next = next;
            _modules = modules;
            _isolation = isolation;
            _proxy = proxy;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = HttpContextUtil.GetMainPath(context);


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

            ResponseEnvolope envolope = await _proxy.HandleRequestAsync(handle, context);





            await _next(context);
        }
    }
}
