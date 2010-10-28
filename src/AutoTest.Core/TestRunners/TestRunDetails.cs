using System;
using System.Collections.Generic;
namespace AutoTest.Core.TestRunners
{
	public class TestRunDetails
	{
		private List<string> _testsToRun;
		
		public TestRunnerType Type { get; private set; }
		public string Assembly { get; private set; }
		public string[] TestsToRun { get { return _testsToRun.ToArray(); } }
		
		public TestRunDetails(TestRunnerType type, string assembly)
		{
			Type = type;
			Assembly = assembly;
			_testsToRun = new List<string>();
		}
		
		public void AddTestToRun(string test)
		{
			_testsToRun.Add(test);
		}
		
		public void AddTestsToRun(string[] tests)
		{
			_testsToRun.AddRange(tests);
		}
	}
}

