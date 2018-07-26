using System;

namespace BeavisCli.DefaultServices
{
    public class DefaultAuthorizationHandler : IAuthorizationHandler
    {
        public virtual bool IsAuthorized(WebCliCommand cmd, WebCliContext context)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // By default every command execution is authorized. If you want some custom 
            // authorization logic, you should
            // - write your own IAuthorizationHandler implementation and register it during the startup or/and
            // - override WebCliCommand.IsAuthorized method

            return true;
        }
    }
}
