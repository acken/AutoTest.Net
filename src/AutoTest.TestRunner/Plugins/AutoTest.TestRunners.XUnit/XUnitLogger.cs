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

        public void AssemblyFinished(string assemblyFilename, int total, int failed, int skipped, double time)
        {
        }

        public void AssemblyStart(string assemblyFilename, string configFilename, string xUnitVersion)
        {
            _currentAssembly = assemblyFilename;
        }

        public bool ClassFailed(string className, string exceptionType, string message, string stackTrace)
        {
            return true;
        }

        public void ExceptionThrown(string assemblyFilename, Exception exception)
        {
        }

        public void TestFailed(string name, string type, string method, double duration, string output, string exceptionType, string message, string stackTrace)
        {
        }

        public bool TestFinished(string name, string type, string method)
        {
            return true;
        }

        public void TestPassed(string name, string type, string method, double duration, string output)
        {
            _results.Add(getResult(duration, name, output));
        }

        private Shared.Results.TestResult getResult(double duration, string name, string message)
        {
            return new Shared.Results.TestResult("XUnit", _currentAssembly, "", duration, name, Shared.Results.TestState.Passed, message);
        }

        public void TestSkipped(string name, string type, string method, string reason)
        {
        }

        public bool TestStart(string name, string type, string method)
        {
            return true;
        }
    }
}
