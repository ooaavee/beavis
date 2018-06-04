using System;
using Beavis.Isolation.Contracts;
using Microsoft.AspNetCore.Http;

namespace Beavis.Isolation.Modules
{
    public class IsolatedModule : IIsolatedModule
    {
        private readonly IServiceProvider _services;
        private readonly ModuleRequestHandlerProvider _handlers;

        public IsolatedModule(IServiceProvider services, ModuleRequestHandlerProvider handlers)
        {
            _services = services;
            _handlers = handlers;
        }

        public ModuleResponse Ping(ModuleRequest request)
        {
            return new ModuleResponse();
        }

        public ModuleResponse HandleRequest(ModuleRequest request)
        {
            HttpContext context = HttpContextUtil.CreateHttpContext(request.HttpRequest, _services);

            ModuleRequestHandler handler = _handlers.GetHandler(context.Request.Path, context.Request.Method);

            if (handler == null)
            {
                // TODO: not found, return 404
            }



            var response = new ModuleResponse();
            response.Data = "REPLY: " + request.Data + " from pipe " + " jokupipe " + " " + DateTime.Now.ToString();
            return response;
        }

        





    }
}
