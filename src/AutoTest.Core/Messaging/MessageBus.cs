using System;
using AutoTest.Core.Configuration;
using System.Threading;
using System.Collections.Generic;

namespace AutoTest.Core.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly IServiceLocator _services;
        private object _padLock = new object();
        private List<BlockedMessage> _blockedMessages = new List<BlockedMessage>();

        public event EventHandler<RunStartedMessageEventArgs> OnRunStartedMessage;
        public event EventHandler<RunFinishedMessageEventArgs> OnRunFinishedMessage;
        public event EventHandler<InformationMessageEventArgs> OnInformationMessage;
        public event EventHandler<WarningMessageEventArgs> OnWarningMessage;
        public event EventHandler<BuildMessageEventArgs> OnBuildMessage;
        public event EventHandler<TestRunMessageEventArgs> OnTestRunMessage;
        public event EventHandler<ErrorMessageEventArgs> OnErrorMessage;
        public event EventHandler<RunInformationMessageEventArgs> OnRunInformationMessage;

        public MessageBus(IServiceLocator services)
        {
            _services = services;
        }

        public void Publish<T>(T message)
        {
            lock (_padLock)
            {
                if (message == null)
                    throw new ArgumentNullException("message");
                if (isBlockingConsumers<T>())
                {
                    withhold(message);
                    return;
                }
                if (hasBlockingConsumers<T>())
                    block<T>();
                ThreadPool.QueueUserWorkItem(tryPublish<T>, message);
            }
        }

        private bool isBlockingConsumers<T>()
        {
            return _blockedMessages.FindIndex(0, m => m.Type.Equals(typeof (T))) >= 0;
        }

        private void withhold(object message)
        {
            var item = _blockedMessages.Find(m => m.Type.Equals(message.GetType()));
            item.Push(message);
        }

        private bool hasBlockingConsumers<T>()
        {
            var consumers = _services.LocateAll<IBlockingConsumerOf<T>>();
            if (consumers == null)
                return false;
            return consumers.Length > 0;
        }

        private void block<T>()
        {
            _blockedMessages.Add(new BlockedMessage(typeof(T)));
        }

        private void tryPublish<T>(object threadContext)
        {
            try
            {
                publish<T>(threadContext);
            }
            catch (Exception exception)
            {
                Publish(new ErrorMessage(exception));
            }
        }

        private void publish<T>(object threadContext)
        {
            T message = (T) threadContext;
            if (handleByType<T>(message))
                return;

            var blockingConsumers = _services.LocateAll<IBlockingConsumerOf<T>>();
            if (blockingConsumers != null && blockingConsumers.Length > 0)
            {
                foreach (var consumer in blockingConsumers)
                    consumer.Consume(message);
            }

            var consumers = _services.LocateAll<IConsumerOf<T>>();
            foreach (var instance in consumers)
                instance.Consume(message);

            publishWithheldMessages<T>();
        }

        private void publishWithheldMessages<T>()
        {
            var item = _blockedMessages.Find(m => m.Type.Equals(typeof (T)));
            if (item ==  null)
                return;

            while (item.HasBlockedMessages)
            {
                publish<T>(item.Pop());
            }
            _blockedMessages.Remove(item);
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
            else if (typeof(T) == typeof(BuildRunMessage))
            {
                if (OnBuildMessage != null)
                    OnBuildMessage(this, new BuildMessageEventArgs((BuildRunMessage) (IMessage) message));
                handled = true;
            }
            else if (typeof(T) == typeof(TestRunMessage))
            {
                if (OnTestRunMessage != null)
                    OnTestRunMessage(this, new TestRunMessageEventArgs((TestRunMessage)(IMessage)message));
                handled = true;
            }
            else if (typeof(T) == typeof(RunStartedMessage))
            {
                if (OnRunStartedMessage != null)
                    OnRunStartedMessage(this, new RunStartedMessageEventArgs((RunStartedMessage)(IMessage)message));
                handled = true;
            }
            else if (typeof(T) == typeof(RunFinishedMessage))
            {
                if (OnRunFinishedMessage != null)
                    OnRunFinishedMessage(this, new RunFinishedMessageEventArgs((RunFinishedMessage)(IMessage)message));
                handled = true;
            }
            else if (typeof(T) == typeof(WarningMessage))
            {
                if (OnWarningMessage != null)
                    OnWarningMessage(this, new WarningMessageEventArgs((WarningMessage)(IMessage)message));
                handled = true;
            }
            else if (typeof(T) == typeof(ErrorMessage))
            {
                if (OnErrorMessage != null)
                    OnErrorMessage(this, new ErrorMessageEventArgs((ErrorMessage)(IMessage)message));
                handled = true;
            }
            else if (typeof(T) == typeof(RunInformationMessage))
            {
                if (OnRunInformationMessage != null)
                    OnRunInformationMessage(this, new RunInformationMessageEventArgs((RunInformationMessage)(IMessage)message));
                handled = true;
            }
            return handled;
        }
    }
}