namespace Beavis.Ipc
{
    public class NamedPipeServerOptions
    {
        public string PipeName { get; set; }
        public int MaxNumberOfServerInstances { get; set; } = 20;
        public int InitialNumberOfServerInstances { get; set; } = 4;
    }
}
