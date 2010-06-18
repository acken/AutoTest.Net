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
        public BuildRunMessage Message { get; private set; }

        public BuildMessageEventArgs(BuildRunMessage runMessage)
        {
            Message = runMessage;
        }
    }

    public class TestRunMessageEventArgs : EventArgs
    {
        public TestRunMessage Message { get; private set; }

        public TestRunMessageEventArgs(TestRunMessage message)
        {
            Message = message;
        }
    }

    public interface IMessageBus
    {
        event EventHandler<InformationMessageEventArgs> OnInformationMessage;
        event EventHandler<BuildMessageEventArgs> OnBuildMessage;
        event EventHandler<TestRunMessageEventArgs> OnTestRunMessage;
        void Publish<T>(T message);
    }
}