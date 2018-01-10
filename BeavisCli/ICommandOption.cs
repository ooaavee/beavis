using System.Collections.Generic;

namespace BeavisCli
{
    public interface ICommandOption
    {
        List<string> Values { get; }

        bool HasValue();

        string Value();
    }
}