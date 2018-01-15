using System;
using System.Collections.Generic;
using System.IO;
using BeavisCli.Internal;
using Newtonsoft.Json;

namespace BeavisCli
{
    public class ApplicationExecutionResponse
    {
        [JsonProperty("messages")]
        public List<ResponseMessage> Messages { get; set; } = new List<ResponseMessage>();

        [JsonProperty("statements")]
        public List<string> Statements { get; set; } = new List<string>();

        /// <summary>
        /// Writes an empty line.
        /// </summary>
        public virtual void WriteEmptyLine()
        {
            Messages.Add(new InformationMessage { Text = string.Empty });
        }

        /// <summary>
        /// Writes an information message.
        /// </summary>
        public virtual void WriteInformation(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            Messages.Add(new InformationMessage { Text = text });
        }

        /// <summary>
        /// Writes a succeed/ very positive message.
        /// </summary>
        public virtual void WriteSucceed(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            Messages.Add(new InformationMessage { Text = Green(text) });
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public virtual void WriteError(Exception e, bool returnStackTrace = false)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            string text = returnStackTrace ? e.ToString() : e.Message;

            Messages.Add(new ErrorMessage { Text = text });
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public virtual void WriteError(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            Messages.Add(new ErrorMessage { Text = text });
        }

        /// <summary>
        /// Returns green text.
        /// </summary>
        private static string Green(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            //terminal.echo('[[;#00ff00;]' + text + ']' + "Tämä on normaalia tekstiä normaalillä värillä!!!");
            return "[[;#00ff00;]" + text + "]";
            
            // + '] ]';
        }

        /// <summary>
        /// Adds a JavaScript statement.
        /// </summary>
        public virtual void AddStatement(IJavaScriptStatement statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            Statements.Add(statement.GetJavaScript());
        }

        /// <summary>
        /// Creates a TextWriter for information messages.
        /// </summary>
        public virtual TextWriter CreateTextWriterForInformationMessages()
        {
            return new MessageContainerTextWriter(WriteInformation);
        }

        /// <summary>
        /// Creates a TextWriter for error messages.
        /// </summary>
        public virtual TextWriter CreateTextWriterForErrorMessages()
        {
            return new MessageContainerTextWriter(WriteError);
        }

    }
}
