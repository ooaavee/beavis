using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Beavis.Ipc
{
    public sealed class BeavisHttpContext : DefaultHttpContext
    {
        public BeavisHttpContext()
        {
        }

        internal BeavisHttpRequest BeavisRequest { get; set; }
        internal BeavisHttpResponse BeavisResponse { get; set; }

        public override HttpRequest Request
        {
            get { return BeavisRequest; }
        }

        public override HttpResponse Response
        {
            get { return BeavisResponse; }
        }

       

        //public static string GetSubPath(HttpContext context)
        //{
        //    var path = context.RequestMessage.Path.ToString();
        //    if (path.Length < 2)
        //    {
        //        return null;
        //    }
        //    var index = path.IndexOf('/', 1);
        //    return index < 0 ? "" : path.Substring(index);
        //}


    }
}