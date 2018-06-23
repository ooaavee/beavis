using System;

namespace Beavis.Ipc
{
    public class PipeMessageEventArgs : EventArgs
    {
        public PipeMessageEventArgs(string request)
        {
            Request = request;
        }

        /// <summary>
        /// Request message
        /// </summary>
        public string Request { get; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Response { get; set; }
    }
}
