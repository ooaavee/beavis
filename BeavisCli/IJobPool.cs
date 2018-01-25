namespace BeavisCli
{
    public interface IJobPool
    {
        string Push(IJob job);
    }
}
