namespace BeavisCli
{
    /// <summary>
    /// Identifies a file in a <see cref="IFileStorage"/>.
    /// </summary>
    public struct FileId
    {
        public FileId(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}