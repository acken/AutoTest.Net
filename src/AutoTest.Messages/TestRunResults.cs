using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoTest.Messages
{
	public class TestRunResults
	{
		private string _project;
        private string _assembly;
        private TimeSpan _timeSpent;
        private readonly TestResult[] _testResults;

        public string Project { get { return _project; } }
        public string Assembly { get { return _assembly; } }
        public TimeSpan TimeSpent { get { return _timeSpent; } }
        public TestResult[] All { get { return _testResults; } }
        public TestResult[] Passed { get { return queryByStatus(TestRunStatus.Passed); } }
        public TestResult[] Failed { get { return queryByStatus(TestRunStatus.Failed); } }
        public TestResult[] Ignored { get { return queryByStatus(TestRunStatus.Ignored); } }

        public TestRunResults(string project, string assembly, TestResult[] testResults)
        {
            _project = project;
            _assembly = assembly;
            _testResults = testResults;
        }

        public void SetTimeSpent(TimeSpan timeSpent)
        {
            _timeSpent = timeSpent;
        }

        private TestResult[] queryByStatus(TestRunStatus status)
        {
            var query = from t in _testResults
                        where t.Status.Equals(status)
                        select t;
            return query.ToArray();
        }
	}
}

