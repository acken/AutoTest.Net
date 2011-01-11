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
        private Options options;
        private string currentAssembly = "";
        private ArrayList unhandledExceptions = new ArrayList();
        private List<AutoTest.TestRunners.Shared.TestResult> _results = new List<AutoTest.TestRunners.Shared.TestResult>();

        public IEnumerable<AutoTest.TestRunners.Shared.TestResult> Results { get { return _results; } }

        public TestHarvester(Options options)
        {
            this.options = options;
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
                    var result = new AutoTest.TestRunners.Shared.TestResult("nunit", currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Test.TestName.FullName, TestRunners.Shared.TestState.Failed, testResult.Message);
                    result.AddStackLines(getStackLines(testResult).ToArray());
                    _results.Add(result);
                    break;

                case ResultState.Inconclusive:
                case ResultState.Success:
                    _results.Add(new AutoTest.TestRunners.Shared.TestResult("nunit", currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Test.TestName.FullName, TestRunners.Shared.TestState.Passed, testResult.Message));
                    break;

                case ResultState.Ignored:
                case ResultState.Skipped:
                case ResultState.NotRunnable:
                    var ignoreResult = new AutoTest.TestRunners.Shared.TestResult("nunit", currentAssembly, getFixture(testResult.Test.TestName.FullName), testResult.Test.TestName.FullName, TestRunners.Shared.TestState.Ignored, testResult.Message);
                    ignoreResult.AddStackLines(getStackLines(testResult).ToArray());
                    _results.Add(ignoreResult);
                    break;
            }
        }

        private static IEnumerable<TestRunners.Shared.StackLine> getStackLines(TestResult testResult)
        {
            var stackLines = new List<TestRunners.Shared.StackLine>();
            string stackTrace = StackTraceFilter.Filter(testResult.StackTrace);
            if (stackTrace != null && stackTrace != string.Empty)
            {
                string[] trace = stackTrace.Split(System.Environment.NewLine.ToCharArray());
                foreach (string s in trace)
                {
                    if (s != string.Empty)
                        stackLines.Add(new TestRunners.Shared.StackLine(s));
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
            _results.Add(new AutoTest.TestRunners.Shared.TestResult("nunit", currentAssembly, "", "Unhandled exception", TestRunners.Shared.TestState.Panic, exception.ToString()));
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
