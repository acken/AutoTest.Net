using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.Messaging.TestClasses
{
    class InformationMessageConsumer
    {
        public bool InformationMessageEventWasCalled = false;
        public bool WarningMessageEventWasCalled = false;

        public InformationMessageConsumer(IMessageBus bus)
        {
            bus.OnInformationMessage += new EventHandler<InformationMessageEventArgs>(bus_OnInformationMessage);
            bus.OnWarningMessage += new EventHandler<WarningMessageEventArgs>(bus_OnWarningMessage);
        }

        void bus_OnInformationMessage(object sender, InformationMessageEventArgs e)
        {
            InformationMessageEventWasCalled = true;
        }

        void bus_OnWarningMessage(object sender, WarningMessageEventArgs e)
        {
            WarningMessageEventWasCalled = true;
        }
    }
}
