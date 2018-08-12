using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeavisCli.Commands
{
    [Command("shortcuts", "Keyboard shortcuts")]
    public class Shortcuts : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            await context.OnExecuteAsync(() =>
            {
                var items = new List<ShortcutsModel>
                {
                    new ShortcutsModel("TAB", "Tab completion is available or tab character."),
                    new ShortcutsModel("Shift+Enter", "Insert new line."),
                    new ShortcutsModel("Up Arrow/CTRL+P", "Show previous command from history."),
                    new ShortcutsModel("Down Arrow/CTRL+N", "Show next command from history."),
                    new ShortcutsModel("CTRL+R", "Reverse Search through command line history."),
                    new ShortcutsModel("CTRL+G", "Cancel Reverse Search."),
                    new ShortcutsModel("CTRL+L", "Clear terminal."),
                    new ShortcutsModel("CTRL+Y", "Paste text from kill area."),
                    new ShortcutsModel("Delete/backspace", "Remove one character from right/left to the cursor."),
                    new ShortcutsModel("Left Arrow/CTRL+B", "Move left."),
                    new ShortcutsModel("Right Arrow/CTRL+F", "Move right."),
                    new ShortcutsModel("CTRL+Left Arrow", "Move one word to the left."),
                    new ShortcutsModel("CTRL+Right Arrow", "Move one word to the right."),
                    new ShortcutsModel("CTRL+A/Home", "Move to beginning of the line."),
                    new ShortcutsModel("CTRL+E/End", "Move to end of the line."),
                    new ShortcutsModel("CTRL+K", "Remove the text after the cursor and save it in kill area."),
                    new ShortcutsModel("CTRL+U", "Remove the text before the cursor and save it in kill area."),
                    new ShortcutsModel("CTRL+V/SHIFT+Insert", "Insert text from system clipboard."),
                    new ShortcutsModel("CTRL+W", "Remove text to the begining of the word (don't work in Chrome)."),
                    new ShortcutsModel("CTRL+H", "Remove text to the end of the line."),
                    new ShortcutsModel("ALT+D", "Remove one word after the cursor (don't work in IE)."),
                    new ShortcutsModel("PAGE UP", "Scroll up (don't work in Chrome)."),
                    new ShortcutsModel("PAGE DOWN", "Scroll down (don't work in Chrome).")
                };

                context.WriteText("Keyboard shortcuts:");

                string[] lines = LineFormatter.FormatLines(items, x => x.Keys, x => x.Description, true, true);

                foreach (string line in lines)
                {
                    context.WriteText(line);
                }

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
