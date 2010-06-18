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
        private InformationMessage _informationmessage = null;
        private WarningMessage _warningMessage = null;

        public string InformationMessage { get { return _informationmessage.Message; } }
        public string WarningMessage { get { return _warningMessage.Warning; } }

        #region IInformationFeedbackView Members

        public void RecievingInformationMessage(InformationMessage message)
        {
            _informationmessage = message;
        }

        public void RecievingWarningMessage(WarningMessage message)
        {
            _warningMessage = message;
        }

        #endregion
    }
}
