using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem;

namespace AutoTest.Test.Core.Caching.Projects.Fakes
{
    class FakeFileSystemService : IFileSystemService
    {
        private string _searchPattern = "";
        private string[] _projectFiles = null;

        public FakeFileSystemService WhenCrawlingFor(string searchPattern)
        {
            _searchPattern = searchPattern;
            return this;
        }

        public void Return(string projectFiles)
        {
            _projectFiles = new string[] {projectFiles};
        }

        #region IFileSystemService Members

        public string[] GetFiles(string path, string searchPattern)
        {
            if (searchPattern.Equals(_searchPattern))
                return _projectFiles;
            return new string[] {};
        }

        public string ReadFileAsText(string path)
        {
            return new FileSystemService().ReadFileAsText(path);
        }

        #endregion
    }
}
