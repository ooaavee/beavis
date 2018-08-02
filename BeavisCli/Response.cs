using BeavisCli.JavaScriptStatements;
using BeavisCli.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public class Response
    {
        private readonly HttpContext _httpContext;

        internal Response(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        /// <summary>
        /// Occurs just before sending the response.
        /// </summary>
        public event EventHandler Sending;

        /// <summary>
        /// Response messages
        /// </summary>
        [JsonProperty("messages")]
        public List<ResponseMessage> Messages { get; } = new List<ResponseMessage>();

        /// <summary>
        /// JavaScript statements that will be evaluated on the client-side.
        /// </summary>
        [JsonProperty("statements")]
        public List<string> Statements { get; } = new List<string>();

        /// <summary>
        /// Writes an empty line.
        /// </summary>
        public void WriteEmptyLine()
        {
            Messages.Add(new InformationMessage(string.Empty));
        }
              
        /// <summary>
        /// Writes an information message.
        /// </summary>
        public void WriteInformation(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            Messages.Add(new InformationMessage(text));
        }

        /// <summary>
        /// Writes an information message.
        /// </summary>
        public void WriteInformation(IEnumerable<string> texts)
        {
            if (texts == null)
            {
                throw new ArgumentNullException(nameof(texts));
            }

            foreach (string text in texts)
            {
                WriteInformation(text);
            }
        }

        /// <summary>
        /// Writes a success/ very positive message.
        /// </summary>
        public void WriteSuccess(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            Messages.Add(new SuccessMessage(text));
        }

        /// <summary>
        /// Writes a success/ very positive message.
        /// </summary>
        public void WriteSuccess(IEnumerable<string> texts)
        {
            if (texts == null)
            {
                throw new ArgumentNullException(nameof(texts));
            }

            foreach (string text in texts)
            {
                WriteSuccess(text);
            }
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public void WriteError(Exception e, bool returnStackTrace = false)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            string text = returnStackTrace ? e.ToString() : e.Message;

            WriteError(text);
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public void WriteError(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            Messages.Add(new ErrorMessage(text));
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public void WriteError(IEnumerable<string> texts)
        {
            if (texts == null)
            {
                throw new ArgumentNullException(nameof(texts));
            }

            foreach (string text in texts)
            {
                WriteError(text);
            }
        }

        /// <summary>
        /// Adds a JavaScript statement that will be invoked on the client-side.
        /// </summary>
        public void AddJavaScript(IJavaScriptStatement js)
        {
            if (js == null)
            {
                throw new ArgumentNullException(nameof(js));
            }

            string code = js.GetCode();

            Statements.Add(code);   
        }

        public void AddJavaScript(IEnumerable<IJavaScriptStatement> js)
        {
            if (js == null)
            {
                throw new ArgumentNullException(nameof(js));
            }

            foreach (IJavaScriptStatement j in js)
            {
                AddJavaScript(j);
            }
        }

        public void WriteFile(byte[] data, string fileName, string mimeType)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (fileName == null)
            {
                throw  new ArgumentNullException(nameof(fileName));
            }

            if (mimeType == null)
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            IJob job = new WriteFileJob(data, fileName, mimeType);

            AddJob(job);
        }

        public void AddJob(IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            // this will be invoked just before we are sending the response
            Sending += (sender, args) =>
            {
                // push a new job into the pool and add a JavaScript statement that
                // begins the job on the client-side
                IJobPool pool = _httpContext.RequestServices.GetRequiredService<IJobPool>();
                string key = pool.Push(job);
                IJavaScriptStatement js = new BeginJob(key);
                AddJavaScript(js);
            };
        }

        /// <summary>
        /// Raises the <see cref="Sending"/> event.
        /// </summary>
        internal void OnSending()
        {
            EventHandler handler = Sending;

            if (handler != null)
            {
                lock (handler)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}
