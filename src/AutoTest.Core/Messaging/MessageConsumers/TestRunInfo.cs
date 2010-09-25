using System;
using AutoTest.Core.Caching.Projects;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class TestRunInfo
	{
		public Project Project { get; private set; }
		public string Assembly { get; private set; }
		
		public TestRunInfo(Project project, string assembly)
		{
			Project = project;
			Assembly = assembly;
		}
	}
}

