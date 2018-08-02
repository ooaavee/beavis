using System;

namespace BeavisCli.Services
{
    public class AuthorizationHandler : IAuthorizationHandler
    {
        public virtual bool IsAuthorized(Command cmd, CommandContext context)
        {          
            // By default every command execution is authorized. If you want some custom 
            // authorization logic, you should
            // - write your own IAuthorizationHandler implementation and register it during the startup or/and
            // - override WebCliCommand.IsAuthorized method

            return true;
        }
    }
}
