using System;
using System.Threading.Tasks;

namespace BeavisCli.Services
{
    public class DefaultUnauthorizedHandler : IUnauthorizedHandler
    {
        public virtual Task OnUnauthorizedAsync(CommandContext context)
        {
            context.WriteError("Unauthorized");
            return Task.CompletedTask;
        }
    }
}
