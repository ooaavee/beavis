using BeavisCli.JavaScriptStatements;
using BeavisCli.Jobs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace BeavisCli
{
    public static class ResponseExtensions
    {
        /// <summary>
        /// Writes an empty line.
        /// </summary>
        public static void WriteEmptyLine(this Response response)
        {
            response.Messages.Add(new InformationMessage(string.Empty));
        }

        /// <summary>
        /// Writes an information message.
        /// </summary>
        public static void WriteInformation(this Response response, string text)
        {
            response.Messages.Add(new InformationMessage(text));
        }

        /// <summary>
        /// Writes an information message.
        /// </summary>
        public static void WriteInformation(this Response response, IEnumerable<string> texts)
        {
            foreach (string text in texts)
            {
                response.WriteInformation(text);
            }
        }

        /// <summary>
        /// Writes a success/ very positive message.
        /// </summary>
        public static void WriteSuccess(this Response response, string text)
        {
            response.Messages.Add(new SuccessMessage(text));
        }

        /// <summary>
        /// Writes a success/ very positive message.
        /// </summary>
        public static void WriteSuccess(this Response response, IEnumerable<string> texts)
        {
            foreach (string text in texts)
            {
                response.WriteSuccess(text);
            }
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public static void WriteError(this Response response, Exception e, bool returnStackTrace = false)
        {
            string text = returnStackTrace ? e.ToString() : e.Message;

            response.WriteError(text);
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public static void WriteError(this Response response, string text)
        {           
            response.Messages.Add(new ErrorMessage(text));
        }

        /// <summary>
        /// Writes an error message.
        /// </summary>
        public static void WriteError(this Response response, IEnumerable<string> texts)
        {            
            foreach (string text in texts)
            {
                response.WriteError(text);
            }
        }

        /// <summary>
        /// Adds a JavaScript statement that will be invoked on the client-side.
        /// </summary>
        public static void AddJavaScript(this Response response, IJavaScriptStatement js)
        {            
            string code = js.GetCode();

            response.Statements.Add(code);
        }

        public static void AddJavaScript(this Response response, IEnumerable<IJavaScriptStatement> js)
        {            
            foreach (IJavaScriptStatement j in js)
            {
                response.AddJavaScript(j);
            }
        }

        public static void WriteFile(this Response response, byte[] data, string fileName, string mimeType)
        {            
            IJob job = new WriteFileJob(data, fileName, mimeType);

            response.AddJob(job);
        }

        public static void AddJob(this Response response, IJob job)
        {            
            // this will be invoked just before we are sending the response
            response.Sending += (sender, args) =>
            {
                // push a new job into the pool and add a JavaScript statement that
                // begins the job on the client-side
                IJobPool pool = response.HttpContext.RequestServices.GetRequiredService<IJobPool>();
                string key = pool.Push(job);
                IJavaScriptStatement js = new Job(key);
                response.AddJavaScript(js);
            };
        }


        /*
         *
         *
         *  TODO: Tänne ne mitä on ReponseRenderer luokassa
         *
         *
         */

    }
}
