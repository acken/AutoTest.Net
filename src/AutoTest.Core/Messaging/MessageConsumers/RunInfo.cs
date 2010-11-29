using System;
using System.Linq;
using AutoTest.Core.Caching.Projects;
using System.Collections.Generic;
using AutoTest.Messages;
namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class TestToRun
    {
        public TestRunner Runner { get; private set; }
        public string Test { get; private set; }

        public TestToRun(TestRunner runner, string test)
        {
            Runner = runner;
            Test = test;
        }
    }

	public class RunInfo
	{
        private List<TestToRun> _testsToRun;
        private List<TestRunner> _onlyRunTestsFor;
        private List<TestRunner> _rerunAllWhenFinishedFor;

		public Project Project { get; private set; }
		public bool ShouldBeBuilt { get; private set; }
		public string Assembly { get; private set; }
		
		public RunInfo(Project project)
		{
			Project = project;
			ShouldBeBuilt = false;
			Assembly = null;
            _testsToRun = new List<TestToRun>();
            _onlyRunTestsFor = new List<TestRunner>();
            _rerunAllWhenFinishedFor = new List<TestRunner>();
		}
		
		public void ShouldBuild()
		{
			ShouldBeBuilt = true;
		}
		
		public void SetAssembly(string assembly)
		{
			Assembly = assembly;
		}

        public void AddTestsToRun(TestRunner runner, string[] tests)
        {
            foreach (var test in tests)
                AddTestsToRun(runner, test);
        }

        public void AddTestsToRun(TestRunner runner, string test)
        {
            _testsToRun.Add(new TestToRun(runner, test));
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

        public TestRunInfo CloneToTestRunInfo()
        {
            var runInfo = new TestRunInfo(Project, Assembly);
            runInfo.AddTestsToRun(GetTests());
            foreach (var runner in _onlyRunTestsFor)
                runInfo.ShouldOnlyRunSpcifiedTestsFor(runner);
            foreach (var runner in _rerunAllWhenFinishedFor)
                runInfo.ShouldRerunAllTestWhenFinishedFor(runner);
            return runInfo;
        }
	}
}

