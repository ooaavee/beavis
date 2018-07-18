using BeavisCli.Internal;
using BeavisCli.Internal.Applications;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace BeavisCli
{
    public sealed partial class WebCliOptions
    {
        private IReadOnlyDictionary<string, DefaultApplicationBehaviour> InitDefaultTypes()
        {
            Dictionary<string, DefaultApplicationBehaviour> behaviours = new Dictionary<string, DefaultApplicationBehaviour>();

            void Use<TWebCliApplication>() where TWebCliApplication : WebCliApplication
            {
                WebCliApplicationInfo info = WebCliApplicationInfo.Parse<TWebCliApplication>();

                DefaultApplicationBehaviour behaviour = new DefaultApplicationBehaviour
                {
                    IsVisibleForHelp = true,
                    Enabled = true,
                    Type = typeof(TWebCliApplication)
                };

                behaviours[info.Name] = behaviour;
            }

            Use<Help>();
            Use<Clear>();
            Use<Reset>();
            Use<Shortcuts>();
            Use<License>();
            Use<Upload>();

            return behaviours;
        }
    }
}
