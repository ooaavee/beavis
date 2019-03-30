using System.Security.Claims;
using System.Threading.Tasks;
using BeavisCli;
using BeavisCli.Services;
using DemoWebApp.Models;
using DemoWebApp.Services;
using Microsoft.AspNetCore.Authentication;

namespace DemoWebApp.Commands
{
    [Command("login", "This demo command is used for login.")]
    public class Login : ICommand
    {
        // this is our user repository in this demo
        private readonly IUserRepository _repository;

        // we use this service to initilize the terminal after successful login
        private readonly ITerminalInitializer _initializer;

        public Login(IUserRepository repository, ITerminalInitializer initializer)
        {
            _repository = repository;
            _initializer = initializer;
        }

        public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
        {
            IOption username = builder.Option("-u", "Username");
            IOption password = builder.Option("-p", "Password");

            await context.OnExecuteAsync(async () =>
            {
                if (!(username.HasValue() && password.HasValue()))
                {
                    return await context.ExitWithHelp();
                }

                // find the user by using the UserService service
                UserModel user = _repository.GetUser(username.Value());

                // check password
                if (user == null || user.Password != password.Value())
                {
                    return await context.ExitWithErrorAsync("Invalid username or password.");
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
                return await context.ExitAsync($"Hello {user.UserName}!", ResponseMessageType.Success);
            });
        }
    }
}
