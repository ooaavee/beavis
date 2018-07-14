using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisCli
{
    public interface IAuthorizationHandler
    {
        bool IsAuthorized(WebCliApplication application, WebCliContext context);
    }
}
