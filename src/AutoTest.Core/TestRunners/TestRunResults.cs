using System.Linq;
using System.Collections.Generic;

namespace AutoTest.Core.TestRunners
{
    public class TestRunResults
    {
        private string _project;
        private string _assembly;
        private readonly TestResult[] _testResults;

        public string Project { get { return _project; } }
        public string Assembly { get { return _assembly; } }
        public TestResult[] All { get { return _testResults; } }
        public TestResult[] Passed { get { return queryByStatus(TestStatus.Passed); } }
        public TestResult[] Failed { get { return queryByStatus(TestStatus.Failed); } }
        public TestResult[] Ignored { get { return queryByStatus(TestStatus.Ignored); } }

        public TestRunResults(string project, string assembly, TestResult[] testResults)
        {
            _project = project;
            _assembly = assembly;
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