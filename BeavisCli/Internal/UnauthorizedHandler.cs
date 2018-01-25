namespace BeavisCli.Internal
{
    /// <summary>
    /// Handler for unauthorized application execution attempts.
    /// </summary>
    internal class UnauthorizedHandler : IUnauthorizedHandler
    {
        public void HandleUnauthorizedApplicationExecution(WebCliContext context)
        {
            context.Response.WriteError("Unauthorized");
        }
    }
}
