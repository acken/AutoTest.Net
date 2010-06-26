using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;

namespace AutoTest.Core.Presenters
{
    public interface IRunFeedbackView
    {
        void RecievingRunStartedMessage(RunStartedMessage message);
        void RecievingRunFinishedMessage(RunFinishedMessage message);
        void RecievingBuildMessage(BuildRunMessage message);
        void RecievingTestRunMessage(TestRunMessage message);
        void RecievingRunInformationMessage(RunInformationMessage message);
    }
}
