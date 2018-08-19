using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Demo.Commands
{
    [Command("test", "test")]
    public class Test : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            await context.OnExecuteAsync(() =>
            {

                return context.AskPassword("Please enter your password:", OnPasswordEntered);
            });
        }

        private Task<CommandResult> OnPasswordEntered(CommandSegmentInput input, CommandContext context)
        {
            string password = input.GetString();


            return context.Exit();
        }
    }
}
