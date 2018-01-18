using System.Collections.Generic;

namespace BeavisCli
{
    public interface IArgument
    {
        List<string> Values { get; }

        bool MultipleValues { get; }

        string Value { get; }
    }
}