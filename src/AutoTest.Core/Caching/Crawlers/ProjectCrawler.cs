using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.FileSystem;
using System.IO;

namespace AutoTest.Core.Caching.Crawlers
{
    class ProjectCrawler
    {
        private const string CSHARP_PROJECT_EXTENTION = "csproj";
        private const string VB_PROJECT_EXTENTION = "vbproj";
        private ICache _cache;
        private IFileSystemService _fsService;

        public ProjectCrawler(ICache cache, IFileSystemService fsService)
        {
            _cache = cache;
            _fsService = fsService;
        }

        public void Crawl(string path)
        {
            if (!_fsService.DirectoryExists(path))
                return;
            getCSharpProjects(path);
            getVisualBasicProjects(path);
        }

        private void getCSharpProjects(string path)
        {
            var files = _fsService.GetFiles(
                path,
                string.Format("*.{0}", CSHARP_PROJECT_EXTENTION));
            addProjects(files);
        }

        private void getVisualBasicProjects(string path)
        {
            var files = _fsService.GetFiles(
                path, 
                string.Format("*.{0}", VB_PROJECT_EXTENTION));
            addProjects(files);
        }

        private void addProjects(string[] files)
        {
            foreach (var file in files)
                _cache.Add<Project>(Path.GetFullPath(file));
        }
    }
}
