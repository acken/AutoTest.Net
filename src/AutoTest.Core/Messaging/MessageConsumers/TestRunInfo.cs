using System;
using System.Linq;
using AutoTest.Core.Caching.Projects;
using System.Collections.Generic;
using AutoTest.Messages;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class TestRunInfo
	{
		private List<TestToRun> _testsToRun;
        private List<TestRunner> _onlyRunTestsFor;
        private List<TestRunner> _rerunAllWhenFinishedFor;
		
		public Project Project { get; private set; }
		public string Assembly { get; private set; }
		
		public TestRunInfo(Project project, string assembly)
		{
			Project = project;
			Assembly = assembly;
            _testsToRun = new List<TestToRun>();
            _onlyRunTestsFor = new List<TestRunner>();
            _rerunAllWhenFinishedFor = new List<TestRunner>();
		}

        public void AddTestsToRun(TestToRun[] tests)
        {
            _testsToRun.AddRange(tests);
        }

        public bool OnlyRunSpcifiedTestsFor(TestRunner runner)
        {
            if (_onlyRunTestsFor.Contains(TestRunner.All))
                return true;
            return _onlyRunTestsFor.Contains(runner);
        }

        public void ShouldOnlyRunSpcifiedTestsFor(TestRunner runner)
        {
            if (!_onlyRunTestsFor.Exists(r => r.Equals(runner)))
                _onlyRunTestsFor.Add(runner);
        }

        public bool RerunAllTestWhenFinishedFor(TestRunner runner)
        {
            if (_rerunAllWhenFinishedFor.Contains(TestRunner.All))
                return true;
            return _rerunAllWhenFinishedFor.Contains(runner);
        }

        public void ShouldRerunAllTestWhenFinishedFor(TestRunner runner)
        {
            if (!_rerunAllWhenFinishedFor.Contains(runner))
                _rerunAllWhenFinishedFor.Add(runner);
        }

        public TestToRun[] GetTests()
        {
            return _testsToRun.ToArray();
        }

        public string[] GetTestsFor(TestRunner runner)
        {
            var query = from t in _testsToRun
                        where t.Runner.Equals(runner) || t.Runner.Equals(TestRunner.All)
                        select t.Test;
            return query.ToArray();
        }

        public bool RerunAllTestWhenFinishedForAny()
        {
            return _rerunAllWhenFinishedFor.Count > 0;
        }
	}
}

