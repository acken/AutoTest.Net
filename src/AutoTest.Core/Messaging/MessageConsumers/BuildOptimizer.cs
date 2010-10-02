using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	class BuildOptimizer
	{
		private ICache _cache;
		
		public BuildOptimizer(ICache cache)
		{
			_cache = cache;
		}
		
		public RunInfo[] AssembleBuildConfiguration(string[] projectList)
		{
			var runList = new List<RunInfo>();
			foreach ( var project in projectList)
				runList.Add(new RunInfo(_cache.Get<Project>(project)));
			var shouldBuild = runList.Where<RunInfo>(r => r.Project.Value.ReferencedBy.Length.Equals(0)).ToArray();
			foreach (var item in shouldBuild)
				item.ShouldBuild();
			return runList.ToArray();
		}
	}
}

