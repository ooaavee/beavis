using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Services
{
    public interface ICommandExecutionEnvironment
    {
        /// <summary>
        /// Checks if the command execution is authorized.
        /// </summary>
        /// <param name="context">command context</param>
        /// <returns>true if the command execution is authorized, otherwise false</returns>
        Task<bool> IsAuthorizedAsync(CommandContext context);

        /// <summary>
        /// Checks if the command should be visible to help.
        /// </summary>
        /// <param name="cmd">command</param>
        /// <param name="context">command context</param>
        /// <returns>true if the command should be visible to help, otherwise false</returns>
        bool IsVisibleForHelp(ICommand cmd, HttpContext context);

        /// <summary>
        /// Checks if tab completion should be enabled for the command.
        /// </summary>
        /// <param name="cmd">command</param>
        /// <param name="context">command context</param>
        /// <returns>true if tab completion should be enabled for the command, otherwise false</returns>
        bool IsTabCompletionEnabled(ICommand cmd, HttpContext context);
    }
}
