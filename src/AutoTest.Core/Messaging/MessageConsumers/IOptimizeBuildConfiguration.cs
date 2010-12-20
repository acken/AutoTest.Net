using System;
using AutoTest.Core.Caching.Projects;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public interface IOptimizeBuildConfiguration
	{
		RunInfo[] AssembleBuildConfiguration(string[] projectList, bool useBuiltProjectsOutputPath);
        RunInfo[] AssembleBuildConfiguration(Project[] projectList, bool useBuiltProjectsOutputPath);
	}
}

