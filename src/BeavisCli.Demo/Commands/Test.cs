using System.Threading.Tasks;
using BeavisCli;

namespace DemoWebApp.Commands
{
    [Command("test", "test")]
    public class Test : ICommand
    {
        public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
        {
            await context.OnExecuteAsync(() =>
            {
                return context.AskPassword("Please enter your password:", OnPasswordEntered);
            });
        }

        private Task<CommandResult> OnPasswordEntered(string result, CommandContext context)
        {
            context.WriteSuccess("Dudiin!!!!");

            return context.AskConfirmation("Are you sure?", OnCccc);
        }

        private Task<CommandResult> OnCccc(bool result, CommandContext context)
        {
            if (result)
            {
                context.WriteSuccess("Cool");
                return context.AskText("What you want to do?", Handler);
            }
            else
            {
                context.WriteText("Blaah");
                return context.AskText("Why not?", Handler);
            }


        }

        private Task<CommandResult> Handler(string result, CommandContext context)
        {
            context.WriteText($"Your answer was: {result}");

            return context.Exit();
        }
    }
}
