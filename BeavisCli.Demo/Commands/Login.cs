using BeavisCli.Demo.Models;
using BeavisCli.Demo.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeavisCli.Demo.Commands
{
    [Command("login", "Proceed login")]
    [CommandDescription("Use this....")]
    public class Login : Command
    {
        private readonly UserService _service;
        private readonly ITerminalBehaviour _terminal;

        public Login(UserService service, ITerminalBehaviour terminal)
        {
            _service = service;
            _terminal = terminal;
        }

        public override async Task ExecuteAsync(CommandContext context)
        {
            ICommandOption username = context.Option("-u", "Username", CommandOptionType.SingleValue);
            ICommandOption password = context.Option("-p", "Password", CommandOptionType.SingleValue);

            await OnExecuteAsync(async () =>
            {
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    return await ErrorAsync(context, "You are already autheticated, logout first.");
                }

                if (!(username.HasValue() && password.HasValue()))
                {
                    return await ErrorAsync(context, "Please enter username and password.");
                }

                UserModel user = _service.GetUser(username.Value());

                // check password
                if (user == null || user.Password != password.Value())
                {
                    return await ErrorAsync(context, "Invalid username or password.");
                }

                // sign-in!
                ClaimsIdentity identity = new ClaimsIdentity("password");
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await context.HttpContext.SignInAsync("Demo", principal);
                context.HttpContext.User = principal;

                // tell the terminal to evaluate JS statements provided by the ITerminalBehaviour service -> this way
                // we can modify the terminal look-and-feel for authenticated users
                context.Response.AddJavaScript(_terminal.EnumInitStatements(context.HttpContext));

                return await ExitAsync(context, $"Hello {user.UserName}!", ResponseMessageTypes.Success);
            }, context);
        }
    }
}
