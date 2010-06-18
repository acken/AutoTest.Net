using System;
using AutoTest.Core.Configuration;
using System.Threading;
using System.Collections.Generic;

namespace AutoTest.Core.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly IServiceLocator _services;

        public event EventHandler<InformationMessageEventArgs> OnInformationMessage;
        public event EventHandler<BuildMessageEventArgs> OnBuildMessage;

        public MessageBus(IServiceLocator services)
        {
            _services = services;
        }

        public void Publish<T>(T message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            // TODO: Handle Blocking consumers pr message type
            ThreadPool.QueueUserWorkItem(publish<T>, message);
        }

        private void publish<T>(object threadContext)
        {
            T message = (T) threadContext;
            if (handleByType<T>(message))
                return;
            var instances = _services.LocateAll<IConsumerOf<T>>();
            foreach (var instance in instances)
                instance.Consume(message);
        }

        private bool handleByType<T>(T message)
        {
            bool handled = false;
            if (typeof(T) == typeof(InformationMessage))
            {
                if (OnInformationMessage != null)
                    OnInformationMessage(this, new InformationMessageEventArgs((InformationMessage) (IMessage) message));
                handled = true;
            }
            if (typeof(T) == typeof(BuildRunMessage))
                if (OnBuildMessage != null)
                    OnBuildMessage(this, new BuildMessageEventArgs((BuildRunMessage) (IMessage) message));
            return handled;
        }
    }
}