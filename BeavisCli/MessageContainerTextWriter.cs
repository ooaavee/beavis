using System;
using System.IO;
using System.Text;

namespace BeavisCli
{
    internal class MessageContainerTextWriter : TextWriter
    {
        private readonly Action<string> _action;

        public MessageContainerTextWriter(Action<string> action)
        {
            _action = action;
        }

        public override void WriteLine()
        {
            _action(string.Empty);
        }

        public override void WriteLine(string value)
        {
            _action(value);
        }

        public override void Write(char value)
        {
            throw new NotImplementedException("Sheeeeet! This method is not implemented, because we should not be here :(");
        }

        public override Encoding Encoding => Encoding.UTF8;
    }
}