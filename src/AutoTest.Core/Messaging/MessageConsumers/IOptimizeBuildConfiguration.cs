using System;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public interface IOptimizeBuildConfiguration
	{
		RunInfo[] AssembleBuildConfiguration(string[] projectList);
	}
}

