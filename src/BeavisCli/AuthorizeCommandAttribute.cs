using System;
using System.Threading.Tasks;

namespace BeavisCli
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeCommandAttribute : Attribute
    {
        public virtual Task<bool> IsAuthorizedAsync(CommandContext context)
        {
            return Task.FromResult(true);
        }
    }
}