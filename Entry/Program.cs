using System;
using System.Diagnostics;
using Beavis.Shared;

namespace Beavis.Entry
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var contract = StartupContract.FromCommandLineArguments(args);

            Console.Out.WriteLine("DIIBADUU");

            switch (contract.Type)
            {
                case StartupTypes.Host:
                    Host.Startup.Run(contract);
                    break;

                case StartupTypes.Module:
                    Module.Startup.Run(contract);
                    break;
            }
        }
    }
}
