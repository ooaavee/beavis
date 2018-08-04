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
        private readonly ITerminalInitializer _initializer;

        public Login(UserService service, ITerminalInitializer initializer)
        {
            _service = service;
            _initializer = initializer;
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

                // find the user by using the UserService service
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


                _initializer.Initialize(context.Response, context.HttpContext, true);


                return await ExitAsync(context, $"Hello {user.UserName}!", ResponseMessageTypes.Success);
            }, context);
        }
    }
}
