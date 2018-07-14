using System.Threading.Tasks;

namespace BeavisCli.Internal.DefaultServices
{
    internal class DefaultUnauthorizedHandler : IUnauthorizedHandler
    {
        public Task OnUnauthorizedAsync(WebCliContext context)
        {
            context.Response.WriteError("Unauthorized");
            return Task.CompletedTask;
        }
    }
}
