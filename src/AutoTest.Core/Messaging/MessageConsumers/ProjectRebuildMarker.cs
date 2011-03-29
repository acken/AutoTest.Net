using System;
using AutoTest.Messages;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.DebugLog;
namespace AutoTest.Core
{
	class ProjectRebuildMarker : IMarkProjectsForRebuild
	{
		private ICache _cache;
		
		public ProjectRebuildMarker(ICache cache)
		{
			_cache = cache;
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
                {
                    Debug.WriteDebug("Adding and marking project for rebuild " + file.FullName);
                    _cache.Add<Project>(file.FullName);
                    project = _cache.Get<Project>(file.FullName);
                }
                else
                {
                    Debug.WriteDebug("Reloading and marking project for rebuild " + file.FullName);
                    _cache.Reload<Project>(file.FullName);
                    project = _cache.Get<Project>(file.FullName);
                }
				return true;
			}
			return false;
		}
	}
}

