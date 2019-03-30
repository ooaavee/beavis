using System.Threading.Tasks;

namespace BeavisCli.Services
{
    public interface IUnauthorizedHandler
    {
        Task OnUnauthorizedAsync(CommandContext context);
    }
}
