using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class ProjectChangeConsumerTest
    {
        [Test]
        public void Should_be_a_blocking_consumer()
        {
            var consumer = new ProjectChangeConsumer(null, null, null);
            consumer.ShouldBeOfType<IBlockingConsumerOf<ProjectChangeMessage>>();
        }
    }
}
