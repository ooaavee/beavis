namespace DemoWebApp.Commands
{
using System.Threading.Tasks;
using BeavisCli;

[Command("hello", "This demo say Hello World!")]
public class Hello : ICommand
{
    public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
    {
        await context.OnExecuteAsync(async () =>
        {
            return await context.ExitAsync("Hello World", ResponseMessageTypes.Success);
        });
    }
}
}
