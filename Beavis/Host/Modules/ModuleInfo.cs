namespace Beavis.Host.Modules
{
    public class ModuleInfo
    {
        public int HitCount { get; set; }

        public string Path { get; set; }

        public string Key { get; set; }


        public void OnSucceedRequest()
        {
            // TODO: increase counter
        }

        public void OnFailedRequest()
        {
            // TODO: increase counter
        }

    }
}
