using System;
using AutoTest.Core.Configuration;

namespace AutoTest.Core.Messaging
{
    public class MessageBus : IMessageBus
    {
        readonly IServiceLocator _services;

        public MessageBus(IServiceLocator services)
        {
            _services = services;
        }


        public void Publish<T>(T message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            //ignore the sender for now...?
            var instances = _services.LocateAll<IConsumerOf<T>>();
            foreach (var instance in instances)
                instance.Consume(message);
        }
    }
}