using System;
using System.Text;

namespace BeavisCli.JavaScriptStatements
{
    /// <summary>
    /// This statements starts a file download by using the download.js library.
    /// </summary>
    /// <remarks>
    /// download.js v4.2, by dandavis; 2008-2016.
    /// http://danml.com/download.html
    /// https://github.com/rndme/download
    /// </remarks>
    public sealed class DownloadJs : IJavaScriptStatement
    {
        private readonly string _js;

        public DownloadJs(byte[] data, string fileName, string mimeType)
        {
            var tmp = new StringBuilder();

            tmp.Append($"var s = atob('{Convert.ToBase64String(data)}'); ");
            tmp.Append("var arr = new Array(s.length); ");
            tmp.Append("for (var i = 0; i < s.length; i++) arr[i] = s.charCodeAt(i); ");           
            tmp.Append($"download(new Blob([new Uint8Array(arr)], {{type: \"{mimeType}\"}}), \"{fileName}\", \"{mimeType}\");");

            _js = tmp.ToString();
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
