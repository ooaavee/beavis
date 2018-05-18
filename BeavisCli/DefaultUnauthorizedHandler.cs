namespace BeavisCli
{
    /// <summary>
    /// Handler for unauthorized application execution attempts.
    /// </summary>
    public class DefaultUnauthorizedHandler : IUnauthorizedHandler
    {
        public void OnUnauthorized(WebCliContext context)
        {
            context.Response.WriteError("Unauthorized");
        }
    }
}
