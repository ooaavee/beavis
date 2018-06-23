using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beavis.Ipc
{
    public class BeavisServerOptions
    {
        public string PipeName { get; set; }

        public bool ReturnStackTrace { get; set; }
    }
}
