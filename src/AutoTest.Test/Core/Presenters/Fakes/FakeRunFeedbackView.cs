using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.Presenters.Fakes
{
    class FakeRunFeedbackView : IRunFeedbackView
    {
        private RunStartedMessage _runStartedMessage = null;
        private RunFinishedMessage _runFinishedMessage = null;
        private BuildRunMessage _buildRunMessage = null;
        private TestRunMessage _testRunMessage = null;

        public RunStartedMessage RunStartedMessage { get { return _runStartedMessage; } }
        public RunFinishedMessage RunFinishedMessage { get { return _runFinishedMessage; } }
        public BuildRunMessage BuildRunMessage { get { return _buildRunMessage; } }
        public TestRunMessage TestRunMessage { get { return _testRunMessage; } }

        #region IRunFeedbackView Members

        public void RecievingBuildMessage(BuildRunMessage runMessage)
        {
            _buildRunMessage = runMessage;
        }

        public void RecievingTestRunMessage(TestRunMessage message)
        {
            _testRunMessage = message;
        }

        public void RecievingRunStartedMessage(RunStartedMessage message)
        {
            _runStartedMessage = message;
        }

        public void RecievingRunFinishedMessage(RunFinishedMessage message)
        {
            _runFinishedMessage = message;
        }

        #endregion
    }
}
