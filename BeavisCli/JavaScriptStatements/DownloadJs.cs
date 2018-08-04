using System;
using System.Text.Encodings.Web;

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
            _js = $"download(\"data:{JavaScriptEncoder.Default.Encode(mimeType)};base64,{Convert.ToBase64String(data)}\", \"{JavaScriptEncoder.Default.Encode(fileName)}\", \"{JavaScriptEncoder.Default.Encode(mimeType)}\");";
        }

        public string GetCode()
        {
            return _js;
        }
    }
}
