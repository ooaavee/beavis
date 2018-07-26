using System;
using System.Collections.Generic;
using BeavisCli.Internal;
using BeavisCli.Internal.Commands;

namespace BeavisCli
{
    public sealed class WebCliOptions
    {
        public WebCliOptions()
        {
            BuiltInCommands = GetBuiltInCommands();
        }

        public string Path { get; set; } = "/beaviscli";

        public bool DisplayExceptions { get; set; }

        public Type UnauthorizedHandlerType { get; set; }

        public Type TerminalBehaviourType { get; set; }

        public Type FileStorageType { get; set; }

        public Type AuthorizationHandlerType { get; set; }

        public IReadOnlyDictionary<string, BuiltInCommandDefinition> BuiltInCommands { get; }

        private static IReadOnlyDictionary<string, BuiltInCommandDefinition> GetBuiltInCommands()
        {
            var values = new Dictionary<string, BuiltInCommandDefinition>();

            void Init<TWebCliCommand>() where TWebCliCommand : WebCliCommand
            {
                var info = WebCliCommandInfo.FromType(typeof(TWebCliCommand));
                var definition = new BuiltInCommandDefinition {Type = typeof(TWebCliCommand)};
                values[info.Name] = definition;
            }

            Init<Help>();
            Init<Clear>();
            Init<Reset>();
            Init<Shortcuts>();
            Init<License>();
            Init<Upload>();
            Init<FileStorage>();

            return values;
        }
    }
}