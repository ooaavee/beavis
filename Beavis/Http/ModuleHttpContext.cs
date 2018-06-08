using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Beavis.Isolation.Contracts;
using Microsoft.AspNetCore.Http;

namespace Beavis.Http
{
    public sealed class ModuleHttpContext : DefaultHttpContext, IDisposable
    {
        private readonly ModuleHttpRequest _request;
        private readonly ModuleHttpResponse _response;

        public ModuleHttpContext()
        {
            _request = new ModuleHttpRequest(this);
            _response = new ModuleHttpResponse(this);
        }

        public override HttpRequest Request => _request;

        public override HttpResponse Response => _response;


        public void Dispose()
        {
        }



        public static ModuleHttpContext Create(HttpRequestEnvelope envelope, IServiceProvider serviceProvider)
        {
            ModuleHttpContext context = new ModuleHttpContext();
            //context.User = null       tämä pitää kaivaa jostakin


            context.RequestServices = serviceProvider;



            return context;
        }

        public static HttpRequestEnvelope ParseRequest(HttpContext context)
        {
            HttpRequest request = context.Request;

            var content = new HttpRequestEnvelope();

            content.Method = request.Method;
            content.Scheme = request.Scheme;
            content.IsHttps = request.IsHttps;
            content.Host = request.Host.ToString();
            content.PathBase = request.PathBase.ToString();
            content.Path = GetModulePath(context);
            content.QueryString = request.QueryString.ToString();

            foreach (var kvp in request.Query)
            {
                content.Query[kvp.Key] = kvp.Value.ToArray();
            }

            content.Protocol = request.Protocol;

            foreach (var kvp in request.Headers)
            {
                content.Headers[kvp.Key] = kvp.Value.ToArray();
            }

            foreach (var kvp in request.Cookies)
            {
                content.Cookies.Add(new List<string> { kvp.Key, kvp.Value }.ToArray());
            }

            content.ContentLength = request.ContentLength;
            content.ContentType = request.ContentType;

            var buffer = new byte[1024];
            using (var ms = new MemoryStream())
            {
                int readBytes;
                while ((readBytes = request.Body.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, readBytes);
                }
                content.Body = ms.ToArray();
            }

            content.HasFormContentType = request.HasFormContentType;

            if (context.Request.HasFormContentType)
            {
                //
                // TODO: Should we throw a NotImplementedException() ????
                //


                foreach (var kvp in request.Form)
                {
                    content.Form[kvp.Key] = kvp.Value.ToArray();
                }
            }

            return content;
        }

        public static async Task WriteResponseAsync(HttpResponseEnvelope content, HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "text/plain";

            string text = "Hello World " + DateTime.Now.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

        }

        public static string GetMainPath(HttpContext context)
        {
            var fullPath = context.Request.Path.ToString();
            if (fullPath.Length < 2)
            {
                return null;
            }
            var separatorIndex = fullPath.IndexOf('/', 1);
            return separatorIndex < 0 ? fullPath : fullPath.Substring(0, separatorIndex);
        }

        public static string GetModulePath(HttpContext context)
        {
            var fullPath = context.Request.Path.ToString();
            if (fullPath.Length < 2)
            {
                return null;
            }
            var separatorIndex = fullPath.IndexOf('/', 1);
            return separatorIndex < 0 ? "" : fullPath.Substring(separatorIndex);
        }

    }
}