using BeavisCli.Internal.Jobs;
using BeavisCli.JavaScriptStatements;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public class WebCliResponse
    {
        private readonly HttpContext _context;

        internal WebCliResponse(HttpContext context)
        {
            _context = context;
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
        /// Writes an error message.
        /// </summary>
        public void WriteError(Exception e, bool returnStackTrace = false)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            string text = returnStackTrace ? e.ToString() : e.Message;

            Messages.Add(new ErrorMessage(text));
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
        /// Adds a JavaScript statement.
        /// </summary>
        public void AddStatement(IJavaScriptStatement statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            string js = statement.GetJavaScript();

            Statements.Add(js);
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

            AddJob(new WriteFileJob(data, fileName, mimeType));
        }

        public void AddJob(IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            // This will be invoked just before we are sending the response.
            Sending += (sender, args) =>
            {
                // 1. Get the IJobPool.
                var pool = _context.RequestServices.GetRequiredService<IJobPool>();

                // 2. Push job.
                var key = pool.Push(job);

                // 3. Add a JavaScript statement that begins the job on the client-side.
                AddStatement(new BeginJob(key));
            };
        }

        /// <summary>
        /// Raises the <see cref="Sending"/> event.
        /// </summary>
        internal void OnSending()
        {
            var evt = Sending;
            if (evt != null)
            {
                lock (evt)
                {
                    evt(this, EventArgs.Empty);
                }
            }
        }

    }
}
