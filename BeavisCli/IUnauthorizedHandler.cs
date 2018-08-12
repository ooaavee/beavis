using System.Threading.Tasks;

namespace BeavisCli
{
    public interface IUnauthorizedHandler
    {
        Task OnUnauthorizedAsync(CommandContext context);
    }
}
