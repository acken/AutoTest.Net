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
        private BuildRunMessage _runMessage = null;

        public BuildRunMessage RunMessage { get { return _runMessage; } }

        #region IRunFeedbackView Members

        public void RecievingBuildMessage(BuildRunMessage runMessage)
        {
            _runMessage = runMessage;
        }

        #endregion
    }
}
