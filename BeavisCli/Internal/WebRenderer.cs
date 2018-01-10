using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeavisCli.Internal
{
    internal class WebRenderer
    {
        public string GetTerminalHtml()
        {
            string path = @"C:\Projects\beavis-cli\BeavisCli\Resources\index.html";

            string content = File.ReadAllText(path);


            return content;
        }

        public string GetTerminalJs()
        {
            return "";
        }

        public string GetTerminalCss()
        {
            return "";
        }

    }
}
