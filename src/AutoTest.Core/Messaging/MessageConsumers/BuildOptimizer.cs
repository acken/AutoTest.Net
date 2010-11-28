using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using System.IO;
using AutoTest.Core.Configuration;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	class BuildOptimizer : IOptimizeBuildConfiguration
	{
		private ICache _cache;
		private IConfiguration _configuration;
		
		public BuildOptimizer(ICache cache, IConfiguration configuration)
		{
			_cache = cache;
			_configuration = configuration;
		}
		
		public RunInfo[] AssembleBuildConfiguration(string[] projectList)
		{
			var runList = getRunInfoList(projectList);
			markProjectsForBuild(runList);
			locateAssemblyDestinations(runList);
			return runList.ToArray();
		}
		
		private List<RunInfo> getRunInfoList(string[] projectList)
		{
			var runList = new List<RunInfo>();
			foreach ( var project in projectList)
				runList.Add(new RunInfo(_cache.Get<Project>(project)));
			return runList;
		}

        private void markProjectsForBuild(List<RunInfo> runList)
        {
            var shouldBuild = runList.Where<RunInfo>(r => r.Project.Value.ReferencedBy.Length.Equals(0)).ToArray();
            foreach (var item in shouldBuild)
                item.ShouldBuild();
        }

        private void locateAssemblyDestinations(List<RunInfo> runList)
        {
            for (int i = runList.Count - 1; i >= 0; i--)
            {
                var item = runList[i];
                item.SetAssembly(item.Project.GetAssembly(_configuration.CustomOutputPath));
            }
        }
	}
}

