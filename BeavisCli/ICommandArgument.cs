using System.Collections.Generic;

namespace BeavisCli
{
    public interface ICommandArgument
    {
        List<string> Values { get; }

        bool MultipleValues { get; }

        string Value { get; }
    }
}