using System.Linq;
using System.Collections.Generic;

namespace AutoTest.Core.TestRunners
{
    public class TestRunResults
    {
        readonly TestResult[] _testResults;

        public TestResult[] All { get { return _testResults; } }
        public TestResult[] Passed { get { return queryByStatus(TestStatus.Passed); } }
        public TestResult[] Failed { get { return queryByStatus(TestStatus.Failed); } }
        public TestResult[] Ignored { get { return queryByStatus(TestStatus.Ignored); } }

        public TestRunResults(TestResult[] testResults)
        {
            _testResults = testResults;
        }

        private TestResult[] queryByStatus(TestStatus status)
        {
            var query = from t in _testResults
                        where t.Status.Equals(status)
                        select t;
            return query.ToArray();
        }
    }
}