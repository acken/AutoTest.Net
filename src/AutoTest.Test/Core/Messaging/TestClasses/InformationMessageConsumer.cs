using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.Messaging.TestClasses
{
    class InformationMessageConsumer
    {
        public bool EventWasCalled = false;

        public InformationMessageConsumer(IMessageBus bus)
        {
            bus.OnInformationMessage += new EventHandler<InformationMessageEventArgs>(bus_OnInformationMessage);
        }

        void bus_OnInformationMessage(object sender, InformationMessageEventArgs e)
        {
            EventWasCalled = true;
        }
    }
}
