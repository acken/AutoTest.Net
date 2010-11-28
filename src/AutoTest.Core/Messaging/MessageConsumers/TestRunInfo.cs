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
		
		public Project Project { get; private set; }
		public string Assembly { get; private set; }
        public bool OnlyRunSpcifiedTests { get; private set; }
		public bool RerunAllWhenFinished { get; private set; }
		
		public TestRunInfo(Project project, string assembly)
		{
			Project = project;
			Assembly = assembly;
            _testsToRun = new List<TestToRun>();
            OnlyRunSpcifiedTests = false;
			RerunAllWhenFinished = false;
		}

        public void AddTestsToRun(TestToRun[] tests)
        {
            _testsToRun.AddRange(tests);
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

