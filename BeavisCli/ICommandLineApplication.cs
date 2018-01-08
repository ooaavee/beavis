namespace BeavisCli
{
    public interface ICommandLineApplication
    {
        ICommandOption Option(string template, string description, CommandOptionType optionType);
        ICommandArgument Argument(string name, string description, bool multipleValues = false);
    }
}