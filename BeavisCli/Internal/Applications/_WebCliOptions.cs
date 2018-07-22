using BeavisCli.Internal;
using BeavisCli.Internal.Applications;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace BeavisCli
{
    public sealed partial class WebCliOptions
    {
        private static IReadOnlyDictionary<string, BuiltInApplicationBehaviour> GetBuiltInApplications()
        {
            var values = new Dictionary<string, BuiltInApplicationBehaviour>();

            void Use<TWebCliApplication>() where TWebCliApplication : WebCliApplication
            {
                WebCliApplicationInfo info = WebCliApplicationInfo.Parse<TWebCliApplication>();

                values[info.Name] = new BuiltInApplicationBehaviour { Type = typeof(TWebCliApplication) };
            }

            Use<Help>();
            Use<Clear>();
            Use<Reset>();
            Use<Shortcuts>();
            Use<License>();
            Use<Upload>();
            Use<FileStorage>();

            return values;
        }
    }
}
