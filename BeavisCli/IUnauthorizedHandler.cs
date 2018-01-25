namespace BeavisCli
{
    public interface IUnauthorizedHandler
    {
        void OnUnauthorized(WebCliContext context);
    }
}
