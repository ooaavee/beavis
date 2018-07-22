// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeavisCli.Microsoft.Extensions.CommandLineUtils
{
    internal class AnsiConsole
    {
        private AnsiConsole(TextWriter writer, bool useConsoleColor)
        {
            Writer = writer;

            _useConsoleColor = useConsoleColor;
            if (_useConsoleColor)
            {
                OriginalForegroundColor = Console.ForegroundColor;
            }
        }

        private int _boldRecursion;
        private bool _useConsoleColor;

        public static AnsiConsole GetOutput(bool useConsoleColor)
        {
            return new AnsiConsole(Console.Out, useConsoleColor);
        }

        public static AnsiConsole GetError(bool useConsoleColor)
        {
            return new AnsiConsole(Console.Error, useConsoleColor);
        }

        public TextWriter Writer { get; }

        public ConsoleColor OriginalForegroundColor { get; }

        private void SetColor(ConsoleColor color)
        {
            Console.ForegroundColor = (ConsoleColor)(((int)Console.ForegroundColor & 0x08) | ((int)color & 0x07));
        }

        private void SetBold(bool bold)
        {
            _boldRecursion += bold ? 1 : -1;
            if (_boldRecursion > 1 || (_boldRecursion == 1 && !bold))
            {
                return;
            }

            Console.ForegroundColor = (ConsoleColor)((int)Console.ForegroundColor ^ 0x08);
        }

        public void WriteLine(string message)
        {
            if (!_useConsoleColor)
            {
                Writer.WriteLine(message);
                return;
            }

            var escapeScan = 0;
            for (; ; )
            {
                var escapeIndex = message.IndexOf("\x1b[", escapeScan);
                if (escapeIndex == -1)
                {
                    var text = message.Substring(escapeScan);
                    Writer.Write(text);
                    break;
                }
                else
                {
                    var startIndex = escapeIndex + 2;
                    var endIndex = startIndex;
                    while (endIndex != message.Length &&
                        message[endIndex] >= 0x20 &&
                        message[endIndex] <= 0x3f)
                    {
                        endIndex += 1;
                    }

                    var text = message.Substring(escapeScan, escapeIndex - escapeScan);
                    Writer.Write(text);
                    if (endIndex == message.Length)
                    {
                        break;
                    }

                    switch (message[endIndex])
                    {
                        case 'm':
                            int value;
                            if (int.TryParse(message.Substring(startIndex, endIndex - startIndex), out value))
                            {
                                switch (value)
                                {
                                    case 1:
                                        SetBold(true);
                                        break;
                                    case 22:
                                        SetBold(false);
                                        break;
                                    case 30:
                                        SetColor(ConsoleColor.Black);
                                        break;
                                    case 31:
                                        SetColor(ConsoleColor.Red);
                                        break;
                                    case 32:
                                        SetColor(ConsoleColor.Green);
                                        break;
                                    case 33:
                                        SetColor(ConsoleColor.Yellow);
                                        break;
                                    case 34:
                                        SetColor(ConsoleColor.Blue);
                                        break;
                                    case 35:
                                        SetColor(ConsoleColor.Magenta);
                                        break;
                                    case 36:
                                        SetColor(ConsoleColor.Cyan);
                                        break;
                                    case 37:
                                        SetColor(ConsoleColor.Gray);
                                        break;
                                    case 39:
                                        SetColor(OriginalForegroundColor);
                                        break;
                                }
                            }
                            break;
                    }

                    escapeScan = endIndex + 1;
                }
            }
            Writer.WriteLine();
        }
    }


    internal class CommandArgument
    {
        public CommandArgument()
        {
            Values = new List<string>();
        }

        public string Name { get; set; }
        public bool ShowInHelpText { get; set; } = true;
        public string Description { get; set; }
        public List<string> Values { get; private set; }
        public bool MultipleValues { get; set; }
        public string Value
        {
            get
            {
                return Values.FirstOrDefault();
            }
        }
    }


    internal class CommandLineApplication
    {
        // Indicates whether the parser should throw an exception when it runs into an unexpected argument.
        // If this field is set to false, the parser will stop parsing when it sees an unexpected argument, and all
        // remaining arguments, including the first unexpected argument, will be stored in RemainingArguments property.
        private readonly bool _throwOnUnexpectedArg;

        public CommandLineApplication(bool throwOnUnexpectedArg = true)
        {
            _throwOnUnexpectedArg = throwOnUnexpectedArg;
            Options = new List<CommandOption>();
            Arguments = new List<CommandArgument>();
            Commands = new List<CommandLineApplication>();
            RemainingArguments = new List<string>();
            Invoke = () => 0;
        }

        public CommandLineApplication Parent { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Syntax { get; set; }
        public string Description { get; set; }
        public bool ShowInHelpText { get; set; } = true;
        public readonly List<CommandOption> Options;
        public CommandOption OptionHelp { get; private set; }
        public CommandOption OptionVersion { get; private set; }
        public readonly List<CommandArgument> Arguments;
        public readonly List<string> RemainingArguments;
        public bool IsShowingInformation { get; protected set; }  // Is showing help or version?
        public Func<int> Invoke { get; set; }
        public Func<string> LongVersionGetter { get; set; }
        public Func<string> ShortVersionGetter { get; set; }
        public readonly List<CommandLineApplication> Commands;
        public bool AllowArgumentSeparator { get; set; }
        public TextWriter Out { get; set; } = Console.Out;
        public TextWriter Error { get; set; } = Console.Error;

        public IEnumerable<CommandOption> GetOptions()
        {
            var expr = Options.AsEnumerable();
            var rootNode = this;
            while (rootNode.Parent != null)
            {
                rootNode = rootNode.Parent;
                expr = expr.Concat(rootNode.Options.Where(o => o.Inherited));
            }

            return expr;
        }

        public CommandLineApplication Command(string name, string description, Action<CommandLineApplication> configuration, bool throwOnUnexpectedArg = true)
        {
            var command = new CommandLineApplication(throwOnUnexpectedArg)
            {
                Name = name,
                Description = description,  // Added this line (ooaavee 2016-09-23).
                Parent = this,
                Out = Out,                  // Added this line (ooaavee 2016-09-23).
                Error = Error               // Added this line (ooaavee 2016-09-23).
            };

            Commands.Add(command);
            configuration(command);
            return command;
        }

        public CommandOption Option(string template, string description, CommandOptionType optionType) => Option(template, description, optionType, _ => { }, inherited: false);

        public CommandOption Option(string template, string description, CommandOptionType optionType, bool inherited) => Option(template, description, optionType, _ => { }, inherited);

        public CommandOption Option(string template, string description, CommandOptionType optionType, Action<CommandOption> configuration) => Option(template, description, optionType, configuration, inherited: false);

        public CommandOption Option(string template, string description, CommandOptionType optionType, Action<CommandOption> configuration, bool inherited)
        {
            var option = new CommandOption(template, optionType)
            {
                Description = description,
                Inherited = inherited
            };

            option.LongName = option.ShortName;

            Options.Add(option);
            configuration(option);
            return option;
        }

        public CommandArgument Argument(string name, string description, bool multipleValues = false)
        {
            return Argument(name, description, _ => { }, multipleValues);
        }

        public CommandArgument Argument(string name, string description, Action<CommandArgument> configuration, bool multipleValues = false)
        {
            var lastArg = Arguments.LastOrDefault();
            if (lastArg != null && lastArg.MultipleValues)
            {
                var message = string.Format("The last argument '{0}' accepts multiple values. No more argument can be added.", lastArg.Name);
                throw new InvalidOperationException(message);
            }

            var argument = new CommandArgument { Name = name, Description = description, MultipleValues = multipleValues };
            Arguments.Add(argument);
            configuration(argument);
            return argument;
        }

        public void OnExecute(Func<int> invoke)
        {
            Invoke = invoke;
        }

        // ooaavee, 2017-06-10
        public async Task OnExecuteAsync(Func<int> invoke)
        {
            Invoke = invoke;
        }

        public void OnExecute(Func<Task<int>> invoke)
        {
            Invoke = () => invoke().Result;
        }

        // ooaavee, 2017-06-10
        public async Task OnExecuteAsync(Func<Task<int>> invoke)
        {
            Invoke = () => invoke().Result;
        }

        public int Execute(params string[] args)
        {
            CommandLineApplication command = this;
            CommandOption option = null;
            IEnumerator<CommandArgument> arguments = null;

            for (var index = 0; index < args.Length; index++)
            {
                var arg = args[index];
                var processed = false;
                if (!processed && option == null)
                {
                    string[] longOption = null;
                    string[] shortOption = null;

                    if (arg.StartsWith("--"))
                    {
                        longOption = arg.Substring(2).Split(new[] { ':', '=' }, 2);
                    }
                    else if (arg.StartsWith("-"))
                    {
                        shortOption = arg.Substring(1).Split(new[] { ':', '=' }, 2);
                    }
                    if (longOption != null)
                    {
                        processed = true;
                        var longOptionName = longOption[0];
                        option = command.GetOptions().SingleOrDefault(opt => string.Equals(opt.LongName, longOptionName, StringComparison.Ordinal));

                        if (option == null)
                        {
                            if (string.IsNullOrEmpty(longOptionName) && !command._throwOnUnexpectedArg && AllowArgumentSeparator)
                            {
                                // skip over the '--' argument separator
                                index++;
                            }

                            HandleUnexpectedArg(command, args, index, argTypeName: "option");
                            break;
                        }

                        // If we find a help/version option, show information and stop parsing
                        if (command.OptionHelp == option)
                        {
                            command.ShowHelp();
                            return 0;
                        }
                        else if (command.OptionVersion == option)
                        {
                            command.ShowVersion();
                            return 0;
                        }

                        if (longOption.Length == 2)
                        {
                            if (!option.TryParse(longOption[1]))
                            {
                                command.ShowHint();
                                throw new CommandParsingException(command, $"Unexpected value '{longOption[1]}' for option '{option.LongName}'");
                            }
                            option = null;
                        }
                        else if (option.OptionType == CommandOptionType.NoValue)
                        {
                            // No value is needed for this option
                            option.TryParse(null);
                            option = null;
                        }
                    }
                    if (shortOption != null)
                    {
                        processed = true;
                        option = command.GetOptions().SingleOrDefault(opt => string.Equals(opt.ShortName, shortOption[0], StringComparison.Ordinal));

                        // If not a short option, try symbol option
                        if (option == null)
                        {
                            option = command.GetOptions().SingleOrDefault(opt => string.Equals(opt.SymbolName, shortOption[0], StringComparison.Ordinal));
                        }

                        if (option == null)
                        {
                            HandleUnexpectedArg(command, args, index, argTypeName: "option");
                            break;
                        }

                        // If we find a help/version option, show information and stop parsing
                        if (command.OptionHelp == option)
                        {
                            command.ShowHelp();
                            return 0;
                        }
                        else if (command.OptionVersion == option)
                        {
                            command.ShowVersion();
                            return 0;
                        }

                        if (shortOption.Length == 2)
                        {
                            if (!option.TryParse(shortOption[1]))
                            {
                                command.ShowHint();
                                throw new CommandParsingException(command, $"Unexpected value '{shortOption[1]}' for option '{option.LongName}'");
                            }
                            option = null;
                        }
                        else if (option.OptionType == CommandOptionType.NoValue)
                        {
                            // No value is needed for this option
                            option.TryParse(null);
                            option = null;
                        }
                    }
                }

                if (!processed && option != null)
                {
                    processed = true;
                    if (!option.TryParse(arg))
                    {
                        command.ShowHint();
                        throw new CommandParsingException(command, $"Unexpected value '{arg}' for option '{option.LongName}'");
                    }
                    option = null;
                }

                if (!processed && arguments == null)
                {
                    var currentCommand = command;
                    foreach (var subcommand in command.Commands)
                    {
                        if (string.Equals(subcommand.Name, arg, StringComparison.OrdinalIgnoreCase))
                        {
                            processed = true;
                            command = subcommand;
                            break;
                        }
                    }

                    // If we detect a subcommand
                    if (command != currentCommand)
                    {
                        processed = true;
                    }
                }
                if (!processed)
                {
                    if (arguments == null)
                    {
                        arguments = new CommandArgumentEnumerator(command.Arguments.GetEnumerator());
                    }
                    if (arguments.MoveNext())
                    {
                        processed = true;
                        arguments.Current.Values.Add(arg);
                    }
                }
                if (!processed)
                {
                    HandleUnexpectedArg(command, args, index, argTypeName: "command or argument");
                    break;
                }
            }

            if (option != null)
            {
                command.ShowHint();
                throw new CommandParsingException(command, $"Missing value for option '{option.LongName}'");
            }

            return command.Invoke();
        }

        // Helper method that adds a help option
        public CommandOption HelpOption(string template)
        {
            // Help option is special because we stop parsing once we see it
            // So we store it separately for further use
            OptionHelp = Option(template, "Show help information", CommandOptionType.NoValue);

            return OptionHelp;
        }

        public CommandOption VersionOption(string template, string shortFormVersion, string longFormVersion = null)
        {
            if (longFormVersion == null)
            {
                return VersionOption(template, () => shortFormVersion);
            }
            else
            {
                return VersionOption(template, () => shortFormVersion, () => longFormVersion);
            }
        }

        // Helper method that adds a version option
        public CommandOption VersionOption(string template, Func<string> shortFormVersionGetter, Func<string> longFormVersionGetter = null)
        {
            // Version option is special because we stop parsing once we see it
            // So we store it separately for further use
            OptionVersion = Option(template, "Show version information", CommandOptionType.NoValue);
            ShortVersionGetter = shortFormVersionGetter;
            LongVersionGetter = longFormVersionGetter ?? shortFormVersionGetter;

            return OptionVersion;
        }

        // Show short hint that reminds users to use help option
        public void ShowHint()
        {
            if (OptionHelp != null)
            {
                Out.WriteLine(string.Format("Specify --{0} for a list of available options and commands.", OptionHelp.LongName));
            }
        }

        // Show full help
        public void ShowHelp(string commandName = null)
        {
            for (var cmd = this; cmd != null; cmd = cmd.Parent)
            {
                cmd.IsShowingInformation = true;
            }

            Out.WriteLine(GetHelpText(commandName));
        }

        public virtual string GetHelpText(string commandName = null)
        {
            var headerBuilder = new StringBuilder("Usage:");
            for (var cmd = this; cmd != null; cmd = cmd.Parent)
            {
                headerBuilder.Insert(6, string.Format(" {0}", cmd.Name));
            }

            CommandLineApplication target;

            if (commandName == null || string.Equals(Name, commandName, StringComparison.OrdinalIgnoreCase))
            {
                target = this;
            }
            else
            {
                target = Commands.SingleOrDefault(cmd => string.Equals(cmd.Name, commandName, StringComparison.OrdinalIgnoreCase));

                if (target != null)
                {
                    headerBuilder.AppendFormat(" {0}", commandName);
                }
                else
                {
                    // The command name is invalid so don't try to show help for something that doesn't exist
                    target = this;
                }

            }

            var optionsBuilder = new StringBuilder();
            var commandsBuilder = new StringBuilder();
            var argumentsBuilder = new StringBuilder();

            var arguments = target.Arguments.Where(a => a.ShowInHelpText).ToList();
            if (arguments.Any())
            {
                headerBuilder.Append(" [arguments]");

                argumentsBuilder.AppendLine();
                argumentsBuilder.AppendLine("Arguments:");
                var maxArgLen = arguments.Max(a => a.Name.Length);
                var outputFormat = string.Format("  {{0, -{0}}}{{1}}", maxArgLen + 2);
                foreach (var arg in arguments)
                {
                    argumentsBuilder.AppendFormat(outputFormat, arg.Name, arg.Description);
                    argumentsBuilder.AppendLine();
                }
            }

            var options = target.GetOptions().Where(o => o.ShowInHelpText).ToList();
            if (options.Any())
            {
                headerBuilder.Append(" [options]");

                optionsBuilder.AppendLine();
                optionsBuilder.AppendLine("Options:");
                var maxOptLen = options.Max(o => o.Template.Length);
                var outputFormat = string.Format("  {{0, -{0}}}{{1}}", maxOptLen + 2);
                foreach (var opt in options)
                {
                    optionsBuilder.AppendFormat(outputFormat, opt.Template, opt.Description);
                    optionsBuilder.AppendLine();
                }
            }

            var commands = target.Commands.Where(c => c.ShowInHelpText).ToList();
            if (commands.Any())
            {
                headerBuilder.Append(" [command]");

                commandsBuilder.AppendLine();
                commandsBuilder.AppendLine("Commands:");
                var maxCmdLen = commands.Max(c => c.Name.Length);
                var outputFormat = string.Format("  {{0, -{0}}}{{1}}", maxCmdLen + 2);
                foreach (var cmd in commands.OrderBy(c => c.Name))
                {
                    commandsBuilder.AppendFormat(outputFormat, cmd.Name, cmd.Description);
                    commandsBuilder.AppendLine();
                }

                if (OptionHelp != null)
                {
                    commandsBuilder.AppendLine();
                    commandsBuilder.AppendFormat($"Use \"{target.Name} [command] --{OptionHelp.LongName}\" for more information about a command.");
                    commandsBuilder.AppendLine();
                }
            }

            if (target.AllowArgumentSeparator)
            {
                headerBuilder.Append(" [[--] <arg>...]");
            }

            headerBuilder.AppendLine();

            var nameAndVersion = new StringBuilder();
            nameAndVersion.AppendLine(GetFullNameAndVersion());

            // Added these lines (ooaavee 2017-06-07).
            if (Description != null)
            {
                nameAndVersion.AppendLine(Description);
            }

            nameAndVersion.AppendLine();

            return nameAndVersion.ToString()
                + headerBuilder.ToString()
                + argumentsBuilder.ToString()
                + optionsBuilder.ToString()
                + commandsBuilder.ToString();
        }

        public void ShowVersion()
        {
            for (var cmd = this; cmd != null; cmd = cmd.Parent)
            {
                cmd.IsShowingInformation = true;
            }

            Out.WriteLine(FullName);
            Out.WriteLine(LongVersionGetter());
        }

        public string GetFullNameAndVersion()
        {
            return ShortVersionGetter == null ? FullName : string.Format("{0} {1}", FullName, ShortVersionGetter());
        }

        public void ShowRootCommandFullNameAndVersion()
        {
            var rootCmd = this;
            while (rootCmd.Parent != null)
            {
                rootCmd = rootCmd.Parent;
            }

            Out.WriteLine(rootCmd.GetFullNameAndVersion());
            Out.WriteLine();
        }

        private void HandleUnexpectedArg(CommandLineApplication command, string[] args, int index, string argTypeName)
        {
            if (command._throwOnUnexpectedArg)
            {
                command.ShowHint();
                throw new CommandParsingException(command, $"Unrecognized {argTypeName} '{args[index]}'");
            }
            else
            {
                // All remaining arguments are stored for further use
                command.RemainingArguments.AddRange(new ArraySegment<string>(args, index, args.Length - index));
            }
        }

        private class CommandArgumentEnumerator : IEnumerator<CommandArgument>
        {
            private readonly IEnumerator<CommandArgument> _enumerator;

            public CommandArgumentEnumerator(IEnumerator<CommandArgument> enumerator)
            {
                _enumerator = enumerator;
            }

            public CommandArgument Current
            {
                get
                {
                    return _enumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (Current == null || !Current.MultipleValues)
                {
                    return _enumerator.MoveNext();
                }

                // If current argument allows multiple values, we don't move forward and
                // all later values will be added to current CommandArgument.Values
                return true;
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }
    }

    internal class CommandOption
    {
        public CommandOption(string template, CommandOptionType optionType)
        {
            Template = template;
            OptionType = optionType;
            Values = new List<string>();

            foreach (var part in Template.Split(new[] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (part.StartsWith("--"))
                {
                    LongName = part.Substring(2);
                }
                else if (part.StartsWith("-"))
                {
                    var optName = part.Substring(1);

                    // If there is only one char and it is not an English letter, it is a symbol option (e.g. "-?")
                    if (optName.Length == 1 && !IsEnglishLetter(optName[0]))
                    {
                        SymbolName = optName;
                    }
                    else
                    {
                        ShortName = optName;
                    }
                }
                else if (part.StartsWith("<") && part.EndsWith(">"))
                {
                    ValueName = part.Substring(1, part.Length - 2);
                }
                else
                {
                    throw new ArgumentException($"Invalid template pattern '{template}'", nameof(template));
                }
            }

            if (string.IsNullOrEmpty(LongName) && string.IsNullOrEmpty(ShortName) && string.IsNullOrEmpty(SymbolName))
            {
                throw new ArgumentException($"Invalid template pattern '{template}'", nameof(template));
            }
        }

        public string Template { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string SymbolName { get; set; }
        public string ValueName { get; set; }
        public string Description { get; set; }
        public List<string> Values { get; private set; }
        public CommandOptionType OptionType { get; private set; }
        public bool ShowInHelpText { get; set; } = true;
        public bool Inherited { get; set; }

        public bool TryParse(string value)
        {
            switch (OptionType)
            {
                case CommandOptionType.MultipleValue:
                    Values.Add(value);
                    break;
                case CommandOptionType.SingleValue:
                    if (Values.Any())
                    {
                        return false;
                    }
                    Values.Add(value);
                    break;
                case CommandOptionType.NoValue:
                    if (value != null)
                    {
                        return false;
                    }
                    // Add a value to indicate that this option was specified
                    Values.Add("on");
                    break;
                default:
                    break;
            }
            return true;
        }

        public bool HasValue()
        {
            return Values.Any();
        }

        public string Value()
        {
            return HasValue() ? Values[0] : null;
        }

        private bool IsEnglishLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }
    }

    internal enum CommandOptionType
    {
        MultipleValue,
        SingleValue,
        NoValue
    }

    internal class CommandParsingException : Exception
    {
        public CommandParsingException(CommandLineApplication command, string message)
            : base(message)
        {
            Command = command;
        }

        public CommandLineApplication Command { get; }
    }

}
