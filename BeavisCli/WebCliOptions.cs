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

        public bool UseTerminalInitializer { get; set; } = true;

        public Type UnauthorizedHandlerType { get; set; }

        public Type TerminalInitializerType { get; set; }

        public Type FileStorageType { get; set; }

        public Type AuthorizationHandlerType { get; set; }

        public IReadOnlyDictionary<string, BuiltInCommandBehaviour> BuiltInCommands { get; }

        private static IReadOnlyDictionary<string, BuiltInCommandBehaviour> GetBuiltInCommands()
        {
            var values = new Dictionary<string, BuiltInCommandBehaviour>();

            void Init<TWebCliCommand>() where TWebCliCommand : WebCliCommand
            {
                WebCliCommandInfo info = WebCliCommandInfo.Parse<TWebCliCommand>();

                values[info.Name] = new BuiltInCommandBehaviour { Type = typeof(TWebCliCommand) };
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