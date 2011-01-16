using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using System.Collections;
using NUnit.Util;

namespace AutoTest.TestRunners.NUnit
{
    class TestHarvester : MarshalByRefObject, EventListener
    {
        private string currentAssembly = "";
        private List<AutoTest.TestRunners.Shared.Results.TestResult> _results = new List<AutoTest.TestRunners.Shared.Results.TestResult>();

        public IEnumerable<AutoTest.TestRunners.Shared.Results.TestResult> Results { get { return _results; } }

        public TestHarvester()
        {
        }

        public void RunStarted(string name, int testCount)
        {
            currentAssembly = name;
        }

        public void RunFinished(TestResult result)
        {
        }

        public void RunFinished(Exception exception)
        {
        }

        public void TestFinished(TestResult testResult)
        {
            switch (testResult.ResultState)
            {
                case ResultState.Error:
                case ResultState.Failure:
                case ResultState.Cancelled:
                    var result = new AutoTest.TestRunners.Shared.Results.TestResult("nunit", currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Time, testResult.Test.TestName.FullName, TestRunners.Shared.Results.TestState.Failed, testResult.Message);
                    result.AddStackLines(getStackLines(testResult).ToArray());
                    _results.Add(result);
                    break;

                case ResultState.Inconclusive:
                case ResultState.Success:
                    _results.Add(new AutoTest.TestRunners.Shared.Results.TestResult("nunit", currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Time, testResult.Test.TestName.FullName, TestRunners.Shared.Results.TestState.Passed, testResult.Message));
                    break;

                case ResultState.Ignored:
                case ResultState.Skipped:
                case ResultState.NotRunnable:
                    var ignoreResult = new AutoTest.TestRunners.Shared.Results.TestResult("nunit", currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Time, testResult.Test.TestName.FullName, TestRunners.Shared.Results.TestState.Ignored, testResult.Message);
                    ignoreResult.AddStackLines(getStackLines(testResult).ToArray());
                    _results.Add(ignoreResult);
                    break;
            }
        }

        private static IEnumerable<TestRunners.Shared.Results.StackLine> getStackLines(TestResult testResult)
        {
            var stackLines = new List<TestRunners.Shared.Results.StackLine>();
            string stackTrace = StackTraceFilter.Filter(testResult.StackTrace);
            if (stackTrace != null && stackTrace != string.Empty)
            {
                string[] trace = stackTrace.Split(System.Environment.NewLine.ToCharArray());
                foreach (string s in trace)
                {
                    if (s != string.Empty)
                        stackLines.Add(new TestRunners.Shared.Results.StackLine(s));
                }
            }
            return stackLines;
        }

        private string getFixture(string fullname)
        {
            var end = fullname.LastIndexOf(".");
            if (end == -1)
                return "";
            return fullname.Substring(0, end);
        }

        public void TestStarted(TestName testName)
        {
        }

        public void SuiteStarted(TestName testName)
        {
        }

        public void SuiteFinished(TestResult suiteResult)
        {
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject.GetType() != typeof(System.Threading.ThreadAbortException))
                this.UnhandledException((Exception)e.ExceptionObject);
        }


        public void UnhandledException(Exception exception)
        {
            _results.Add(new AutoTest.TestRunners.Shared.Results.TestResult("nunit", currentAssembly, "", 0, "Unhandled exception", TestRunners.Shared.Results.TestState.Panic, exception.ToString()));
        }

        public void TestOutput(TestOutput output)
        {
        }


        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
