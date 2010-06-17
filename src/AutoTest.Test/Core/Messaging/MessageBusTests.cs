using System;
using System.Reflection;
using AutoTest.Core.Configuration;
using AutoTest.Core.Messaging;
using AutoTest.Test.TestObjects;
using NUnit.Framework;
using Castle.MicroKernel.Registration;
using System.Threading;
using AutoTest.Test.Core.Messaging.TestClasses;

namespace AutoTest.Test.Core.Messaging
{
    [TestFixture]
    public class MessageBusTests : IDisposable
    {
        private readonly IMessageBus _bus; 
        
        public MessageBusTests()
        {
            BootStrapper.Configure();
            BootStrapper.Container
                .Register(Component.For<IConsumerOf<string>>().ImplementedBy<Listener>())
                .Register(Component.For<IConsumerOf<string>>().Forward<IConsumerOf<int>>().ImplementedBy<BigListener>());

            _bus = BootStrapper.Services.Locate<IMessageBus>();
        }

        [Test]
        public void Should_be_able_to_send_message_to_bus_with_no_subscribers()
        {
            _bus.Publish(new Message("hi"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_not_be_able_to_send_null_message()
        {
            _bus.Publish<IMessage>(null);
        }

        [Test]
        public void Should_be_able_to_send_string_message_and_have_it_delivered_to_all_consumers()
        {
            _bus.Publish("Hi");
            waitForAsyncCall();
            Listener.LastMessage.ShouldEqual("Hi");
            BigListener.LastStringMessage.ShouldEqual("Hi");
        }

        [Test]
        public void Should_be_able_to_send_int_message()
        {
            BigListener.LastIntMessage.ShouldEqual(default(int));
            _bus.Publish(100);
            waitForAsyncCall();
            BigListener.LastIntMessage.ShouldEqual(100);
        }

        [Test]
        public void Should_be_able_to_consume_information_message()
        {
            var consumer = new InformationMessageConsumer(_bus);
            var message = new InformationMessage("");
            _bus.Publish<InformationMessage>(message);
            waitForAsyncCall();
            consumer.EventWasCalled.ShouldBeTrue();

        }

        private void waitForAsyncCall()
        {
            Thread.Sleep(20);
        }

        //TODO: need unsubscribe
        public void Dispose()
        {
        }
    }
}