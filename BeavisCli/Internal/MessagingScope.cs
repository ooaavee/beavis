using System.Linq;

namespace BeavisCli.Internal
{
    internal class MessagingScope : IMessagingScope
    {
        private readonly TerminalResponse _response;

        public MessagingScope(TerminalResponse response)
        {
            _response = response;
        }

        public void Dispose()
        {
            if (_response.Messages.Any())
            {

                //_response.Messages.Insert(0, new InformationMessage { Text = string.Empty });

                _response.Messages.Add(new InformationMessage { Text = string.Empty });
            }
        }
    }
}