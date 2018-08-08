using System;
using System.Threading.Tasks;

namespace BeavisCli.Services
{
    public class UnauthorizedHandler : IUnauthorizedHandler
    {
        public virtual Task OnUnauthorizedAsync(CommandContext context)
        {
            context.Response.WriteError("Unauthorized");
            return Task.CompletedTask;
        }
    }
}
