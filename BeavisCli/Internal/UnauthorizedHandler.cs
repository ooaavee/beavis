namespace BeavisCli.Internal
{
    /// <summary>
    /// Handler for unauthorized application execution attempts.
    /// </summary>
    internal class UnauthorizedHandler : IUnauthorizedHandler
    {
        public void OnUnauthorized(WebCliContext context)
        {
            context.Response.WriteError("Unauthorized");
        }
    }
}
