using System;

namespace AutoTest.Core.Messaging
{
    public class InformationMessageEventArgs : EventArgs
    {
        public InformationMessage Message { get; private set; }

        public InformationMessageEventArgs(InformationMessage message)
        {
            Message = message;
        }
    }

    public interface IMessageBus
    {
        event EventHandler<InformationMessageEventArgs> OnInformationMessage;
        void Publish<T>(T message);
    }
}