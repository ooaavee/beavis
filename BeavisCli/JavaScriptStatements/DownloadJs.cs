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
        private readonly byte[] _data;
        private readonly string _fileName;
        private readonly string _mimeType;

        public DownloadJs(byte[] data, string fileName, string mimeType)
        {
            _data = data;
            _fileName = fileName;
            _mimeType = mimeType;
        }

        public string GetJavaScript()
        {
            string base64 = Convert.ToBase64String(_data);
            string js = $"download(\"data:{JavaScriptEncoder.Default.Encode(_mimeType)};base64,{base64}\", \"{JavaScriptEncoder.Default.Encode(_fileName)}\", \"{JavaScriptEncoder.Default.Encode(_mimeType)}\");";
            return js;
        }
    }
}
