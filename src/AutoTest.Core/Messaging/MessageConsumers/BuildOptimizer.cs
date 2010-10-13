using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using System.IO;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	class BuildOptimizer : IOptimizeBuildConfiguration
	{
		private ICache _cache;
		
		public BuildOptimizer(ICache cache)
		{
			_cache = cache;
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
				if (!item.ShouldBeBuilt)
					continue;
				item.SetAssembly(getAssembly(item.Project, item.Project.Value.AssemblyName));
				setAssemblyDestinationsRecursive(runList, item.Project, Path.GetDirectoryName(item.Assembly));
			}
		}
		
		private string getAssembly(Project project, string assemblyName)
		{
			string folder = Path.Combine(Path.GetDirectoryName(project.Key), project.Value.OutputPath);
			return Path.Combine(folder, assemblyName);
		}
		
		private void setAssemblyDestinationsRecursive(List<RunInfo> runList, Project item, string assemblyPath)
		{
			var builtBy = runList.Where<RunInfo>(r => r.Project.Value.ReferencedBy.Contains(item.Key));
			foreach (var project in builtBy)
			{
				if (project.Assembly != null)
					continue;
				project.SetAssembly(Path.Combine(assemblyPath, project.Project.Value.AssemblyName));
				setAssemblyDestinationsRecursive(runList, project.Project, assemblyPath);
			}
		}
	}
}

