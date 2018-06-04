using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using Beavis.Isolation.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Beavis
{
    public static class HttpContextUtil
    {
        public static string GetMainPath(HttpContext context)
        {
            string path = context.Request.Path.ToString();
            if (path.Length < 2)
            {
                return null;
            }
            int index = path.IndexOf('/', 1);
            string route = index < 0 ? path : path.Substring(0, index);
            return route;
        }

        public static string GetModulePath(HttpContext context)
        {
            string path = context.Request.Path.ToString();
            if (path.Length < 2)
            {
                return null;
            }
            int index = path.IndexOf('/', 1);
            string subRoute = index < 0 ? "" : path.Substring(index);
            return subRoute;
        }

        public static HttpRequestEnvelope ParseHttpRequest(HttpContext context)
        {
            HttpRequest request = context.Request;

            var envelope = new HttpRequestEnvelope();

            envelope.Method = request.Method;
            envelope.Scheme = request.Scheme;
            envelope.IsHttps = request.IsHttps;
            envelope.Host = request.Host.ToString();
            envelope.PathBase = request.PathBase.ToString();
            envelope.Path = GetModulePath(context);
            envelope.QueryString = request.QueryString.ToString();

            foreach (var kvp in request.Query)
            {
                envelope.Query[kvp.Key] = kvp.Value.ToArray();
            }

            envelope.Protocol = request.Protocol;

            foreach (var kvp in request.Headers)
            {
                envelope.Headers[kvp.Key] = kvp.Value.ToArray();
            }

            foreach (var kvp in request.Cookies)
            {
                envelope.Cookies.Add(new List<string> { kvp.Key, kvp.Value }.ToArray());
            }

            envelope.ContentLength = request.ContentLength;
            envelope.ContentType = request.ContentType;

            var buffer = new byte[1024];
            using (var ms = new MemoryStream())
            {
                int readBytes;
                while ((readBytes = request.Body.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, readBytes);
                }
                envelope.Body = ms.ToArray();
            }

            envelope.HasFormContentType = request.HasFormContentType;

            if (context.Request.HasFormContentType)
            {
                foreach (var kvp in request.Form)
                {
                    envelope.Form[kvp.Key] = kvp.Value.ToArray();
                }
            }

            return envelope;
        }

        public static HttpContext CreateHttpContext(HttpRequestEnvelope envelope, IServiceProvider serviceProvider)
        {
            CustomHttpContext context = new CustomHttpContext();
            //context.User = null       tämä pitää kaivaa jostakin
            

            context.RequestServices = serviceProvider;



            return context;
        }



        private sealed class CustomHttpContext : DefaultHttpContext
        {
            private readonly CustomHttpRequest _request;
            private readonly CustomHttpResponse _response;

            public CustomHttpContext()
            {
                _request = new CustomHttpRequest(this);
                _response = new CustomHttpResponse(this);
            }

            public override HttpRequest Request => _request;

            public override HttpResponse Response => _response;



        }

        private sealed class CustomHttpRequest : DefaultHttpRequest
        {
            public CustomHttpRequest(CustomHttpContext context) : base(context)
            {

            }
        }

        private sealed class CustomHttpResponse : DefaultHttpResponse
        {
            public CustomHttpResponse(CustomHttpContext context) : base(context)
            {

            }
        }


    }
}