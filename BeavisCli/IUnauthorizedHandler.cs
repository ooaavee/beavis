using System.Threading.Tasks;

namespace BeavisCli
{
    public interface IUnauthorizedHandler
    {
        Task OnUnauthorizedAsync(Command cmd, CommandContext context);
    }
}
