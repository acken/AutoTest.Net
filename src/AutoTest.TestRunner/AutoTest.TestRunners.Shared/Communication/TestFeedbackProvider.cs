using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.Shared.Communication
{
    public interface ITestFeedbackProvider
    {
		void RunStarted();
        void TestStarted(string testName);
        void TestFinished(TestResult result);
		void RunFinished(int testsRan);
    }

    public class NullTestFeedbackProvider : ITestFeedbackProvider
    {
		public void RunStarted()
		{
		}

        public void TestFinished(TestResult result)
        {
        }

        public void TestStarted(string testName)
        {
        }
		
		public void RunFinished(int testsRan)
		{
		}
    }

    public class TestFeedbackProvider : ITestFeedbackProvider
    {
        private SocketClient _channel;

        public TestFeedbackProvider(SocketClient channel)
        {
            _channel = channel;
        }
		
		public void RunStarted()
		{
			_channel.Send("Run started");
		}

        public void TestStarted(string testName)
        {
            if (testName == null)
                return;
            Logger.WriteChunk("\t{0}...", testName);
            _channel.Send("Test started:" + testName);
        }

        public void TestFinished(TestResult result)
        {
            if (result == null)
            {
                Logger.Debug(" - Testresult was null");
                return;
            }
            var xml = result.ToXml();
            if (xml == null)
            {
                Logger.Debug(" - Could not generate xml from " + result.TestName);
                return;
            }
            Logger.Write(" - {0}", result.State.ToString());
            _channel.Send(xml);
        }

		public void RunFinished(int testsRan)
		{
			Logger.Write("Ran {0} tests", testsRan);
			_channel.Send("Run finished:" + testsRan.ToString());
		}
    }
}
