using Beavis.Ipc;
using Beavis.Modules;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Beavis.Middlewares
{
    public class RequestProxyMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ModuleManager _modules;

        public RequestProxyMiddleware(RequestDelegate next, ModuleManager modules)
        {
            _next = next;
            _modules = modules;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.GetModulePath();
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

            BeavisClient client = _modules.GetClient(module);
            if (client == null)
            {
                await _next(context);
                return;
            }

            ModuleResponse response = await client.ProcessRequestAsync(context.Request);

            await WriteResponseAsync(response, context);
        }

        private static async Task WriteResponseAsync(ModuleResponse response, HttpContext context)
        {
            if (response.Succeed)
            {
                context.Response.StatusCode = response.Content.StatusCode;
                context.Response.ContentType = response.Content.ContentType;

                using (var stream = new MemoryStream(response.Content.Body))
                {
                    stream.Position = 0;

                    byte[] data = stream.ToArray();
                    await context.Response.Body.WriteAsync(data, 0, data.Length);
                }

                // TODO: Sovita myös loput responseen!!!


            }
            else
            {

                // TODO: Palauta tässä exception, jos olemassa. Jos ei ole, niin palauta internal server error ->


                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "text/plain";

                string text = "Hello World " + DateTime.Now.ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            }

        }

     

       
    }


}
