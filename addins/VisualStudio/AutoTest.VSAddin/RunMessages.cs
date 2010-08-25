using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTest.VSAddin
{
    class RunMessages
    {
        public RunMessageType Type { get; private set; }
        public string Message { get; private set; }

        public RunMessages(RunMessageType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
