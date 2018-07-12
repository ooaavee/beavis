using System;
using System.Text.Encodings.Web;

namespace BeavisCli.JavaScriptStatements
{
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
            var base64 = Convert.ToBase64String(_data);
            var js = $"download(\"data:{JavaScriptEncoder.Default.Encode(_mimeType)};base64,{base64}\", \"{JavaScriptEncoder.Default.Encode(_fileName)}\", \"{JavaScriptEncoder.Default.Encode(_mimeType)}\");";
            return js;
        }
    }
}
