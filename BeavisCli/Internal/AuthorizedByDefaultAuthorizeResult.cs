namespace BeavisCli.Internal
{
    internal class AuthorizedByDefaultAuthorizeResult : AuthorizeResult
    {
        public AuthorizedByDefaultAuthorizeResult() : base(AuthorizedStatus)
        {            
        }

        public override bool IsAuthorized()
        {
            return true;
        }
    }
}
