using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Messaging;
using Rhino.Mocks;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class ProjectChangeConsumerTest
    {
        private ProjectChangeConsumer _consumer;
        private IMessageBus _bus;

        [SetUp]
        public void SetUp()
        {
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _consumer = new ProjectChangeConsumer(_bus, null, null);
        }

        [Test]
        public void Should_be_a_blocking_consumer()
        {
            _consumer.ShouldBeOfType<IBlockingConsumerOf<ProjectChangeMessage>>();
        }

        [Test]
        public void Should_publish_run_started_message()
        {
            _consumer.Consume(new ProjectChangeMessage());
            _bus.AssertWasCalled(b => b.Publish<RunStartedMessage>(null), b => b.IgnoreArguments());
        }

        [Test]
        public void Should_publish_run_finished_message()
        {
            _consumer.Consume(new ProjectChangeMessage());
            _bus.AssertWasCalled(b => b.Publish<RunFinishedMessage>(null), b => b.IgnoreArguments());
        }
    }
}
