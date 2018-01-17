namespace BeavisCli
{
    public interface IUnauthorizedHandler
    {
        void HandleUnauthorizedApplicationExecution(WebCliContext context);
    }
}
