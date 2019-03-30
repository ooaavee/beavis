using System;
using System.Text;

namespace BeavisCli.JsInterop.Statements
{
    /// <summary>
    /// This statements starts a file download by using the download.js library.
    /// </summary>
    /// <remarks>
    /// download.js v4.2, by dandavis; 2008-2016.
    /// http://danml.com/download.html
    /// https://github.com/rndme/download
    /// </remarks>
    public sealed class DownloadJs : IStatement
    {
        private readonly string _js;

        public DownloadJs(byte[] data, string fileName, string mimeType)
        {
            var s = new StringBuilder();

            s.Append($"var s = atob('{Convert.ToBase64String(data)}'); ");
            s.Append("var arr = new Array(s.length); ");
            s.Append("for (var i = 0; i < s.length; i++) arr[i] = s.charCodeAt(i); ");           
            s.Append($"download(new Blob([new Uint8Array(arr)], {{type: \"{mimeType}\"}}), \"{fileName}\", \"{mimeType}\");");

            _js = s.ToString();
        }

        public string GetJs()
        {
            return _js;
        }
    }
}
