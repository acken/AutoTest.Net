using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;

namespace AutoTest.TestRunners.Shared.Communication
{
    public interface ITestFeedbackProvider
    {
        void TestFinished(TestResult result);
    }

    public class NullTestFeedbackProvider : ITestFeedbackProvider
    {
        public void TestFinished(TestResult result)
        {
        }
    }

    public class TestFeedbackProvider : ITestFeedbackProvider
    {
        private PipeServer _channel;

        public TestFeedbackProvider(PipeServer channel)
        {
            _channel = channel;
        }

        public void TestFinished(TestResult result)
        {
            _channel.Send(result.TestName);
        }
    }
}
