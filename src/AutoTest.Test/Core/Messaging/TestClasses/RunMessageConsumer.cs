using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.Messaging.TestClasses
{
    class RunMessageConsumer
    {
        public bool BuildMessageEventWasCalled = false;
        public bool TestRunMessageEventWasCalled = false;

        public RunMessageConsumer(IMessageBus bus)
        {
            bus.OnBuildMessage += new EventHandler<BuildMessageEventArgs>(bus_OnOnBuildMessage);
            bus.OnTestRunMessage += new EventHandler<TestRunMessageEventArgs>(bus_OnTestRunMessage);
        }

        void bus_OnOnBuildMessage(object sender, BuildMessageEventArgs e)
        {
            BuildMessageEventWasCalled = true;
        }

        void bus_OnTestRunMessage(object sender, TestRunMessageEventArgs e)
        {
            TestRunMessageEventWasCalled = true;
        }
    }
}
