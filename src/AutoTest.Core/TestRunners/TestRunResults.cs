using System.Collections.Generic;

namespace AutoTest.Core.TestRunners
{
    public class TestRunResults
    {
        readonly IEnumerable<TestResult> _testResults;

        public TestRunResults(IEnumerable<TestResult> testResults)
        {
            _testResults = testResults;
        }

        public IEnumerable<TestResult> All
        {
            get { return _testResults; }
        }
    }
}