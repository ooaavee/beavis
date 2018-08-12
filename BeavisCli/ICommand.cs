using System.Threading.Tasks;

namespace BeavisCli
{
    public interface ICommand
    {                 
        Task ExecuteAsync(CommandContext context);           
    }
}