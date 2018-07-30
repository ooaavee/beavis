using BeavisCli.Commands;
using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public sealed class BeavisCliOptions
    {
        public BeavisCliOptions()
        {
            BuiltInCommands = GetBuiltInCommands();
        }

        public string Path { get; set; } = "/beaviscli";

        public bool DisplayExceptions { get; set; }

        public Type UnauthorizedHandlerType { get; set; }

        public Type TerminalBehaviourType { get; set; }

        public Type FileStorageType { get; set; }

        public Type AuthorizationHandlerType { get; set; }

        public IReadOnlyDictionary<string, CommandDefinition> BuiltInCommands { get; }

        private static IReadOnlyDictionary<string, CommandDefinition> GetBuiltInCommands()
        {
            var values = new Dictionary<string, CommandDefinition>();

            void Init<TWebCliCommand>() where TWebCliCommand : Command
            {
                var info = CommandInfo.FromType(typeof(TWebCliCommand));
                var definition = new CommandDefinition {Type = typeof(TWebCliCommand)};
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