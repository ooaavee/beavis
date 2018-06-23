using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Beavis.Ipc
{
    public sealed class BeavisHttpResponse : DefaultHttpResponse, IDisposable
    {
        private readonly BeavisResponseCookies _cookies = new BeavisResponseCookies();

        public BeavisHttpResponse(BeavisHttpContext context) : base(context)
        {
            Body = new MemoryStream();
        }

        public override IResponseCookies Cookies => _cookies;

        public override void Redirect(string location)
        {
            HasRedirect = true;
            RedirectLocation = location;
        }

        public override void Redirect(string location, bool permanent)
        {
            HasRedirect = true;
            RedirectLocation = location;
            IsRedirectPermanent = permanent;
        }

        public bool HasRedirect { get; private set; }
        public string RedirectLocation { get; private set; }
        public bool? IsRedirectPermanent { get; private set; }

        public async Task OnPipelineExceptionAsync(Exception e, bool returnStackTrace)
        {
            string text;

            if (returnStackTrace)
            {
                text = e.ToString();
            }
            else
            {
                text = "An error has occurred on the server.";
            }
            
            ContentType = "text/plain";
            StatusCode = (int)HttpStatusCode.InternalServerError;

            await this.WriteAsync(text, Encoding.UTF8);
        }

        public void Dispose()
        {
        }
    }
}