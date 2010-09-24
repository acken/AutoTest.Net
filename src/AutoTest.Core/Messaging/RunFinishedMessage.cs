using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.Core.Messaging
{
    public class RunFinishedMessage : IMessage
    {
        public RunReport Report { get; private set; }

        public RunFinishedMessage(RunReport report) 
        {
            Report = report;
        }
    }
}
