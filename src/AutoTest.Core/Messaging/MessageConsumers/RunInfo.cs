using System;
using AutoTest.Core.Caching.Projects;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	class RunInfo
	{
		public Project Project { get; private set; }
		public bool ShouldBeBuilt { get; private set; }
		
		public RunInfo(Project project)
		{
			Project = project;
			ShouldBeBuilt = false;
		}
		
		public void ShouldBuild()
		{
			ShouldBeBuilt = true;
		}
	}
}

