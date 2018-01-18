using System.Collections.Generic;

namespace BeavisCli
{
    public interface IOption
    {
        List<string> Values { get; }

        bool HasValue();

        string Value();
    }
}