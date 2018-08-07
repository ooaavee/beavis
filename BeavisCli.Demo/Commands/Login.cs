using BeavisCli.Demo.Models;
using BeavisCli.Demo.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeavisCli.Demo.Commands
{
    [Command("login", "This demo command is used for login.")]
    [CommandDescription("Use this command to authenticate yourself. You can use username \"Beavis\" and password \"Cornholio\" to login.")]
    public class Login : Command
    {
        // this is our user repository in this demo
        private readonly DemoUserRepository _repository;

        // we use this service to initilize the terminal after successful login
        private readonly ITerminalInitializer _initializer;

        public Login(DemoUserRepository repository, ITerminalInitializer initializer)
        {
            _repository = repository;
            _initializer = initializer;
        }

        public override async Task ExecuteAsync(CommandContext context)
        {
            ICommandOption username = context.Option("-u", "Username", CommandOptionType.SingleValue);
            ICommandOption password = context.Option("-p", "Password", CommandOptionType.SingleValue);

            await OnExecuteAsync(async () =>
            {
                if (!(username.HasValue() && password.HasValue()))
                {
                    return await ExitWithHelp(context);
                }

                // find the user by using the UserService service
                UserModel user = _repository.GetUser(username.Value());

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

                // initializer terminal after successful login
                _initializer.Initialize(context.Response, context.HttpContext, true);

                // exit and show simple message
                return await ExitAsync(context,
                                       $"Hello {user.UserName}!", 
                                       ResponseMessageTypes.Success);
            }, context);
        }
    }
}
