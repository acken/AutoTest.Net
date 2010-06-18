using System;
using AutoTest.Core.BuildRunners;

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

    public class BuildMessageEventArgs : EventArgs
    {
        public BuildRunMessage RunMessage { get; private set; }

        public BuildMessageEventArgs(BuildRunMessage runMessage)
        {
            RunMessage = runMessage;
        }
    }

    public interface IMessageBus
    {
        event EventHandler<InformationMessageEventArgs> OnInformationMessage;
        event EventHandler<BuildMessageEventArgs> OnBuildMessage;
        void Publish<T>(T message);
    }
}