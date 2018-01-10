namespace BeavisCli
{
    public struct ApplicationInfo
    {       
        public ApplicationInfo(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Host name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Host description
        /// </summary>
        public string Description { get; }
    }
}
