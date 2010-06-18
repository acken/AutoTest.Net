using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging
{
    public class WarningMessage : IMessage
    {
        public string Warning { get; private set; }

        public WarningMessage(string warning)
        {
            Warning = warning;
        }
    }
}
