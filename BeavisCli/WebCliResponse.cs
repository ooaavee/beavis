using System;
using System.Collections.Generic;
using System.IO;
using BeavisCli.Internal;
using Newtonsoft.Json;

namespace BeavisCli
{
    public class WebCliResponse
    {
        [JsonProperty("messages")]
        public List<ResponseMessage> Messages { get; } = new List<ResponseMessage>();

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

        /////// <summary>
        /////// Creates a TextWriter for information messages.
        /////// </summary>
        ////public TextWriter CreateTextWriterForInformationMessages()
        ////{
        ////    return new ResponseMessageTextWriter(WriteInformation);
        ////}

        /////// <summary>
        /////// Creates a TextWriter for error messages.
        /////// </summary>
        ////public TextWriter CreateTextWriterForErrorMessages()
        ////{
        ////    return new ResponseMessageTextWriter(WriteError);
        ////}

    }
}
