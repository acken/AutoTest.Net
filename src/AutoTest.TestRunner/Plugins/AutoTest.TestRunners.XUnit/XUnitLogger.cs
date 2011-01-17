using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AutoTest.TestRunners.XUnit
{
    class XUnitLogger : IRunnerLogger
    {
        private List<AutoTest.TestRunners.Shared.Results.TestResult> _results = new List<Shared.Results.TestResult>();
        private string _currentAssembly = null;

        public IEnumerable<AutoTest.TestRunners.Shared.Results.TestResult> Results { get { return _results; } }

        public void AssemblyFinished(string assemblyFilename, int total, int failed, int skipped, double time)
        {
        }

        public void AssemblyStart(string assemblyFilename, string configFilename, string xUnitVersion)
        {
            _currentAssembly = assemblyFilename;
        }

        public bool ClassFailed(string className, string exceptionType, string message, string stackTrace)
        {
            _results.Add(getResult(0, Shared.Results.TestState.Panic, className, message, stackTrace));
            return true;
        }

        public void ExceptionThrown(string assemblyFilename, Exception exception)
        {
            _results.Add(getResult(0, Shared.Results.TestState.Panic, "Internal XUnit error", exception.ToString()));
        }

        public void TestFailed(string name, string type, string method, double duration, string output, string exceptionType, string message, string stackTrace)
        {
            _results.Add(getResult(duration, Shared.Results.TestState.Failed, name, message, stackTrace));
        }

        public bool TestFinished(string name, string type, string method)
        {
            return true;
        }

        public void TestPassed(string name, string type, string method, double duration, string output)
        {
            _results.Add(getResult(duration, Shared.Results.TestState.Passed, name, output));
        }

        public void TestSkipped(string name, string type, string method, string reason)
        {
            _results.Add(getResult(0, Shared.Results.TestState.Ignored, name, reason));
        }

        public bool TestStart(string name, string type, string method)
        {
            return true;
        }

        private Shared.Results.TestResult getResult(double duration, Shared.Results.TestState state, string name, string message)
        {
            return getResult(duration, state, name, message, null);
        }

        private Shared.Results.TestResult getResult(double duration, Shared.Results.TestState state, string name, string message, string stackLines)
        {
            var result = new Shared.Results.TestResult("XUnit", _currentAssembly, "", duration * 1000, name, state, message);
            if (stackLines != null)
            {
                var lines = stackLines.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                    result.AddStackLine(new Shared.Results.StackLine(line));
            }
            return result;
        }
    }
}
