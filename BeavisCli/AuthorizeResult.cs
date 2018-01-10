using System;
using System.Threading.Tasks;

namespace BeavisCli
{
    public class AuthorizeResult
    {
        public const string AuthorizedStatus = "Authorized";

        public const string UnauthorizedStatus = "Unauthorized";

        public static readonly AuthorizeResult Authorized = new AuthorizeResult(AuthorizedStatus);

        public static readonly AuthorizeResult Unauthorized = new AuthorizeResult(UnauthorizedStatus);

        public AuthorizeResult(string status)
        {
            if (status == null)
            {
                throw new ArgumentNullException(nameof(status));
            }

            Status = status;
        }

        public string Status { get; }

        public virtual bool IsAuthorized()
        {
            return true;
        }

        public static Task<AuthorizeResult> AuthorizedTask()
        {
            return Task.FromResult(Authorized);
        }

        public static Task<AuthorizeResult> UnauthorizedTask()
        {
            return Task.FromResult(Unauthorized);
        }

    }
}
