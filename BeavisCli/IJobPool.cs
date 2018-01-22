using System;
using System.Collections.Generic;
using System.Text;

namespace BeavisCli
{
    public interface IJobPool
    {
        string Push(IJob job);
    }
}
