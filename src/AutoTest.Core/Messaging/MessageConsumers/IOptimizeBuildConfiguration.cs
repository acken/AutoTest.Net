using System;
using AutoTest.Core.Caching.Projects;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public interface IOptimizeBuildConfiguration
	{
		RunInfo[] AssembleBuildConfiguration(string[] projectList);
        RunInfo[] AssembleBuildConfiguration(Project[] projectList);
	}
}

