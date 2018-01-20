using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeavisCli.Internal.Applications
{  
    internal class Shortcuts : WebCliApplication
    {
        public Shortcuts() : base("shortcuts", "Displays a list of keyboard shortcuts.") { }

        public override async Task ExecuteAsync(WebCliContext context)
        {
            await OnExecuteAsync(() =>
            {
                var shortcuts = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("TAB", "Tab completion is available or tab character."),
                    new Tuple<string, string>("Shift+Enter", "Insert new line."),
                    new Tuple<string, string>("Up Arrow/CTRL+P", "Show previous command from history."),
                    new Tuple<string, string>("Down Arrow/CTRL+N", "Show next command from history."),
                    new Tuple<string, string>("CTRL+R", "Reverse Search through command line history."),
                    new Tuple<string, string>("CTRL+G", "Cancel Reverse Search."),
                    new Tuple<string, string>("CTRL+L", "Clear terminal."),
                    new Tuple<string, string>("CTRL+Y", "Paste text from kill area."),
                    new Tuple<string, string>("Delete/backspace", "Remove one character from right/left to the cursor."),
                    new Tuple<string, string>("Left Arrow/CTRL+B", "Move left."),
                    //new Tuple<string, string>("CTRL+TAB", "Switch to next terminal (use scrolling with animation) (don't work in Chrome)."),
                    new Tuple<string, string>("Right Arrow/CTRL+F", "Move right."),
                    new Tuple<string, string>("CTRL+Left Arrow", "Move one word to the left."),
                    new Tuple<string, string>("CTRL+Right Arrow", "Move one word to the right."),
                    new Tuple<string, string>("CTRL+A/Home", "Move to beginning of the line."),
                    new Tuple<string, string>("CTRL+E/End", "Move to end of the line."),
                    new Tuple<string, string>("CTRL+K", "Remove the text after the cursor and save it in kill area."),
                    new Tuple<string, string>("CTRL+U", "Remove the text before the cursor and save it in kill area."),
                    new Tuple<string, string>("CTRL+V/SHIFT+Insert", "Insert text from system clipboard."),
                    new Tuple<string, string>("CTRL+W", "Remove text to the begining of the word (don't work in Chrome)."),
                    new Tuple<string, string>("CTRL+H", "Remove text to the end of the line."),
                    new Tuple<string, string>("ALT+D", "Remove one word after the cursor (don't work in IE)."),
                    new Tuple<string, string>("PAGE UP", "Scroll up (don't work in Chrome)."),
                    new Tuple<string, string>("PAGE DOWN", "Scroll down (don't work in Chrome).")
                    //new Tuple<string, string>("CTRL+D", "Run previous interpreter from the stack or call logout (if terminal is using authentication and current interpreter is the first one). It also cancel all ajax call, if terminal is paused, and resume it.")
                };

                context.Response.WriteInformation("This is list of keyboard shortcuts:");

                foreach (string text in TerminalUtil.MakeBeautifulLines(shortcuts))
                {
                    context.Response.WriteInformation(text);
                }

                return Exit(context);
            }, context);

        }

    }
}
