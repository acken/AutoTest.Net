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
        private BuildRunMessage _buildRunMessage = null;
        private TestRunMessage _testRunMessage = null;

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

        #endregion
    }
}
