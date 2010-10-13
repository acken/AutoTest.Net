using System;
using AutoTest.Core.Caching.Projects;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class RunInfo
	{
		public Project Project { get; private set; }
		public bool ShouldBeBuilt { get; private set; }
		public string Assembly { get; private set; }
		
		public RunInfo(Project project)
		{
			Project = project;
			ShouldBeBuilt = false;
			Assembly = null;
		}
		
		public void ShouldBuild()
		{
			ShouldBeBuilt = true;
		}
		
		public void SetAssembly(string assembly)
		{
			Assembly = assembly;
		}
	}
}

