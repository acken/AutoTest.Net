using System;
using AutoTest.Core.Caching.Projects;
using System.Collections.Generic;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class RunInfo
	{
        private List<string> _testsToRun;

		public Project Project { get; private set; }
		public bool ShouldBeBuilt { get; private set; }
		public string Assembly { get; private set; }
        public string[] TestsToRun { get { return _testsToRun.ToArray(); } }
		
		public RunInfo(Project project)
		{
			Project = project;
			ShouldBeBuilt = false;
			Assembly = null;
            _testsToRun = new List<string>();
		}
		
		public void ShouldBuild()
		{
			ShouldBeBuilt = true;
		}
		
		public void SetAssembly(string assembly)
		{
			Assembly = assembly;
		}

        public void AddTestsToRun(string[] tests)
        {
            _testsToRun.AddRange(tests);
        }
	}
}

