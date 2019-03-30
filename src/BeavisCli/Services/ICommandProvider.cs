using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace BeavisCli.Services
{
    public interface ICommandProvider
    {
        /// <summary>
        /// Finds a registered command by its name.
        /// </summary>
        /// <param name="name">command name</param>
        /// <param name="context">HTTP context</param>
        /// <returns>command or null if not found</returns>
        ICommand Find(string name, HttpContext context);

        /// <summary>
        /// Gets all registered commands.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>commands</returns>
        IEnumerable<ICommand> All(HttpContext context);
    }
}
