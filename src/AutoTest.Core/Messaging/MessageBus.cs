using System;
using AutoTest.Core.Configuration;
using System.Threading;
using System.Collections.Generic;

namespace AutoTest.Core.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly IServiceLocator _services;

        public MessageBus(IServiceLocator services)
        {
            _services = services;
        }

        public void Publish<T>(T message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            ThreadPool.QueueUserWorkItem(publish<T>, message);
        }

        private void publish<T>(object threadContext)
        {
            T message = (T) threadContext;
            var instances = _services.LocateAll<IConsumerOf<T>>();
            foreach (var instance in instances)
                instance.Consume(message);
        }
    }
}