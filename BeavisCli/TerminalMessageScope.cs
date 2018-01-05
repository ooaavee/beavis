using System;
using System.Linq;

namespace BeavisCli
{
    internal class TerminalMessageScope : ITerminalMessageScope
    {
        private readonly TerminalResponse _response;

        public TerminalMessageScope(TerminalResponse response)
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