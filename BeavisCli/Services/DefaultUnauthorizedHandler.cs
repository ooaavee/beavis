using System;
using System.Threading.Tasks;

namespace BeavisCli.Services
{
    public class DefaultUnauthorizedHandler : IUnauthorizedHandler
    {
        public virtual Task OnUnauthorizedAsync(Command cmd, CommandContext context)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Response.WriteError("Unauthorized");
            return Task.CompletedTask;
        }
    }
}
