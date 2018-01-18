namespace BeavisCli.Internal
{
    internal interface ICommandLineApplication
    {
        IOption Option(string template, string description, CommandOptionType optionType);

        IArgument Argument(string name, string description, bool multipleValues = false);
    }
}