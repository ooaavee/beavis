using System;

namespace Beavis.Ipc
{
    public class PipeMessageEventArgs : EventArgs
    {
        public string RequestMessage { get; set; }
        public string ResponseMessage { get; set; }

        public PipeMessageEventArgs(string requestMessage)
        {
            RequestMessage = requestMessage;
        }
    }
}
