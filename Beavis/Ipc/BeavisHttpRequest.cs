using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace Beavis.Ipc
{
    public sealed class BeavisHttpRequest : DefaultHttpRequest, IDisposable
    {
        public BeavisHttpRequest(BeavisHttpContext context, HttpRequestModel model) : base(context)
        {
            Method = model.Method;
            Scheme = model.Scheme;
            IsHttps = model.IsHttps;
            Host = new HostString(model.Host);
            PathBase = new PathString(model.PathBase);
            Path = new PathString(model.Path);
            QueryString = new QueryString(model.QueryString);

            var query = new Dictionary<string, StringValues>();
            foreach (var item in model.Query)
            {
                query[item.Key] = new StringValues(item.Value);
            }
            Query = new QueryCollection(query);

            Protocol = model.Protocol;

            foreach (var item in model.Headers)
            {
                Headers[item.Key] = new StringValues(item.Value);
            }

            var cookies = new Dictionary<string, string>();
            foreach (var item in model.Cookies)
            {
                cookies[item[0]] = item[1];
            }
            Cookies = new RequestCookieCollection(cookies);

            ContentLength = model.ContentLength;
            ContentType = model.ContentType;
            Body = new MemoryStream(model.Body) {Position = 0};

            //
            // Please note, form content type has not been implemented!
            //
            HasFormContentType = model.HasFormContentType;
            Form = new FormCollection(new Dictionary<string, StringValues>());
        }

        public override bool HasFormContentType { get; }

        public void Dispose()
        {
            Body.Dispose();            
        }
    }
}