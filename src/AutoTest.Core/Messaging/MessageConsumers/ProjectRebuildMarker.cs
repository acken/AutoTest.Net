using System;
using AutoTest.Messages;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
namespace AutoTest.Core
{
	class ProjectRebuildMarker : IMarkProjectsForRebuild
	{
		private ICache _cache;
		private IReload<Project> _reloader;
		
		public ProjectRebuildMarker(ICache cache, IReload<Project> reloader)
		{
			_cache = cache;
			_reloader = reloader;
		}
		
		public void HandleProjects(FileChangeMessage message)
		{
			foreach (var file in message.Files)
			{
				handleProject(".csproj", file);
				handleProject(".vbproj", file);
			}
		}
		
		private bool handleProject(string extension, ChangedFile file)
		{
			if (file.Extension.ToLower().Equals(extension))
			{
				var project = _cache.Get<Project>(file.FullName);
				if (project == null)
					return false;
				_reloader.MarkAsDirty(project);
				project = _cache.Get<Project>(file.FullName);
				project.Value.RebuildOnNextRun();
				return true;
			}
			return false;
		}
	}
}

