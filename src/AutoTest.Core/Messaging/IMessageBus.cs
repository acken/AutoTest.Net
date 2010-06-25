using System;
using AutoTest.Core.BuildRunners;

namespace AutoTest.Core.Messaging
{
    public class RunStartedMessageEventArgs : EventArgs
    {
        public RunStartedMessage Message { get; private set; }

        public RunStartedMessageEventArgs(RunStartedMessage message)
        {
            Message = message;
        }
    }

    public class RunFinishedMessageEventArgs : EventArgs
    {
        public RunFinishedMessage Message { get; private set; }

        public RunFinishedMessageEventArgs(RunFinishedMessage message)
        {
            Message = message;
        }
    }

    public class InformationMessageEventArgs : EventArgs
    {
        public InformationMessage Message { get; private set; }

        public InformationMessageEventArgs(InformationMessage message)
        {
            Message = message;
        }
    }

    public class WarningMessageEventArgs : EventArgs
    {
        public WarningMessage Message { get; private set; }

        public WarningMessageEventArgs(WarningMessage message)
        {
            Message = message;
        }
    }

    public class ErrorMessageEventArgs : EventArgs
    {
        public ErrorMessage Message { get; private set; }

        public ErrorMessageEventArgs(ErrorMessage message)
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
        event EventHandler<RunStartedMessageEventArgs> OnRunStartedMessage;
        event EventHandler<RunFinishedMessageEventArgs> OnRunFinishedMessage;
        event EventHandler<InformationMessageEventArgs> OnInformationMessage;
        event EventHandler<WarningMessageEventArgs> OnWarningMessage;
        event EventHandler<BuildMessageEventArgs> OnBuildMessage;
        event EventHandler<TestRunMessageEventArgs> OnTestRunMessage;
        event EventHandler<ErrorMessageEventArgs> OnErrorMessage;
        void Publish<T>(T message);
    }
}