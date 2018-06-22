using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Beavis.Ipc
{
    public sealed class BeavisHttpResponse : DefaultHttpResponse
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

        public void OnErrr(Exception e)
        {

        }

    }
}