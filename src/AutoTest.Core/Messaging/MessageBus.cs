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
                // TODO: Handle Blocking consumers pr message type
                ThreadPool.QueueUserWorkItem(publish<T>, message);
            }
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