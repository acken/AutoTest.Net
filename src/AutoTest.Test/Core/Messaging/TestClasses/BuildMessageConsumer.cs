using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.Messaging.TestClasses
{
    class BuildMessageConsumer
    {
        public bool EventWasCalled = false;

        public BuildMessageConsumer(IMessageBus bus)
        {
            bus.OnBuildMessage += new EventHandler<BuildMessageEventArgs>(bus_OnOnBuildMessage);
        }

        void bus_OnOnBuildMessage(object sender, BuildMessageEventArgs e)
        {
            EventWasCalled = true;
        }
    }
}
