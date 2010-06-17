using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging
{
    public class InformationMessage : IMessage
    {
        private string _message;

        public string Message { get { return _message; } }

        public InformationMessage(string message)
        {
            _message = message;
        }
    }
}
