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

            //ModuleHandle handle = _modules.GetHandle(module);
            //if (handle == null)
            //{
            //    await _next(context);
            //    return;
            //}

            BeavisClient client = _modules.GetClient(module);
            if (client == null)
            {
                await _next(context);
                return;
            }

            ModuleResponse response = await client.ProcessRequestAsync(context.Request);

            if (response.Succeed)
            {
                await WriteResponseAsync(response.Content, context);
            }
            else
            {
                await WriteResponseAsync(response.Exception, context);
            }
        }


        public static async Task WriteResponseAsync(HttpResponseModel response, HttpContext context)
        {
            context.Response.StatusCode = response.StatusCode;
            context.Response.ContentType = response.ContentType;

            using (MemoryStream stream = new MemoryStream(response.Body))
            {
                stream.Position = 0;

                using (StreamReader reader = new StreamReader(stream))
                {

                    var s = reader.ReadToEnd();

                    var data = s;

                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }

            }

           

        }

        public static async Task WriteResponseAsync(Exception ex, HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/plain";

            string text = "Hello World " + DateTime.Now.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

        }

       
    }


}
