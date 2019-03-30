using System.Threading.Tasks;

namespace BeavisCli
{
    public interface ICommand
    {                 
        Task ExecuteAsync(CommandBuilder builder, CommandContext context);           
    }
}