namespace DemoWebApp.Commands
{
    using System.Threading.Tasks;
    using BeavisCli;

    [Command("hello", "This demo says Hello World!")]
    public class Hello : ICommand
    {
        public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
        {
            await context.OnExecuteAsync(async () =>
            {
                context.WriteText("Hello World");
                return await context.ExitAsync();
            });
        }
    }
}
