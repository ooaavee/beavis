using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("shortcuts", "Displays terminal keyboard shortcuts.")]
    public class Shortcuts : ICommand
    {
        public async Task ExecuteAsync(CommandBuilder builder, CommandContext context)
        {
            await context.OnExecuteAsync(() =>
            {
                var items = new[]
                {
                    new ShortcutsModel("Tab", "Tab completion is available or tab character."),
                    new ShortcutsModel("Shift+Enter", "Insert new line."),
                    new ShortcutsModel("Up Arrow/Ctrl+P", "Show previous command from history."),
                    new ShortcutsModel("Down Arrow/Ctrl+N", "Show next command from history."),
                    new ShortcutsModel("Ctrl+R", "Reverse Search through command line history."),
                    new ShortcutsModel("Ctrl+G", "Cancel Reverse Search."),
                    new ShortcutsModel("Ctrl+L", "Clear terminal."),
                    new ShortcutsModel("Ctrl+Y", "Paste text from kill area."),
                    new ShortcutsModel("Delete/backspace", "Remove one character from right/left to the cursor."),
                    new ShortcutsModel("Left Arrow/Ctrl+B", "Move left."),
                    new ShortcutsModel("Right Arrow/Ctrl+F", "Move right."),
                    new ShortcutsModel("Ctrl+Left Arrow", "Move one word to the left."),
                    new ShortcutsModel("Ctrl+Right Arrow", "Move one word to the right."),
                    new ShortcutsModel("Ctrl+A/Home", "Move to beginning of the line."),
                    new ShortcutsModel("Ctrl+E/End", "Move to end of the line."),
                    new ShortcutsModel("Ctrl+K", "Remove the text after the cursor and save it in kill area."),
                    new ShortcutsModel("Ctrl+U", "Remove the text before the cursor and save it in kill area."),
                    new ShortcutsModel("Ctrl+V/Shift+Insert", "Insert text from system clipboard."),
                    new ShortcutsModel("Ctrl+W", "Remove text to the begining of the word (don't work in Chrome)."),
                    new ShortcutsModel("Ctrl+H", "Remove text to the end of the line."),
                    new ShortcutsModel("Alt+D", "Remove one word after the cursor (don't work in IE)."),
                    new ShortcutsModel("Page Up", "Scroll up (don't work in Chrome)."),
                    new ShortcutsModel("Page Down", "Scroll down (don't work in Chrome).")
                };

                context.WriteObjects(items, x => x.Keys, x => x.Description, false, true);

                return context.Exit();
            });
        }

        private class ShortcutsModel
        {
            public ShortcutsModel(string keys, string description)
            {
                Keys = keys;
                Description = description;
            }

            public string Keys { get;  }
            public string Description { get; }
        }
    }
}
