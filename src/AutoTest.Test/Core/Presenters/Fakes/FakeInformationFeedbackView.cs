using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.Presenters.Fakes
{
    class FakeInformationFeedbackView : IInformationFeedbackView
    {
        private InformationMessage _message = null;

        public string Message { get { return _message.Message; } }

        #region IInformationFeedbackView Members

        public void RecievingInformationMessage(InformationMessage message)
        {
            _message = message;
        }

        #endregion
    }
}
