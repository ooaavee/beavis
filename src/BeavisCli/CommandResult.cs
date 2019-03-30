namespace BeavisCli
{
    public struct CommandResult
    {
        public static readonly CommandResult Default = new CommandResult {StatusCode = 2};

        public int StatusCode { get; set; }
    }
}
