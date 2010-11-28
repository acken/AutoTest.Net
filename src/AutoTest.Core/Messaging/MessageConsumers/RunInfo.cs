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

		public Project Project { get; private set; }
		public bool ShouldBeBuilt { get; private set; }
		public string Assembly { get; private set; }
        public bool OnlyRunSpcifiedTests { get; private set; }
		public bool RerunAllWhenFinished { get; private set; }
		
		public RunInfo(Project project)
		{
			Project = project;
			ShouldBeBuilt = false;
			Assembly = null;
            _testsToRun = new List<TestToRun>();
            OnlyRunSpcifiedTests = false;
			RerunAllWhenFinished = false;
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

        public void ShouldOnlyRunSpcifiedTests()
        {
            OnlyRunSpcifiedTests = true;
        }
		
		public void RerunAllTestWhenFinished()
		{
			RerunAllWhenFinished = true;
		}

        public TestToRun[] GetTests()
        {
            return _testsToRun.ToArray();
        }

        public string[] GetTestsFor(TestRunner runner)
        {
            var query = from t in _testsToRun
                        where t.Runner.Equals(runner) || t.Runner.Equals(TestRunner.Unknown)
                        select t.Test;
            return query.ToArray();
        }
	}
}

