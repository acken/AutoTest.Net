using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Core.FileSystem.ProjectLocators
{
    class VisualBasicLocator : ILocateProjects
    {
        private ICrawlForProjectFiles _filesLocator;

        public VisualBasicLocator(ICrawlForProjectFiles filesLocator)
        {
            _filesLocator = filesLocator;
        }

        #region ILocateProjects Members

        public ChangedFile[] Locate(string file)
        {
            return _filesLocator.FindParent(Path.GetDirectoryName(file), ".vbproj");
        }

        public bool IsProject(string file)
        {
            return Path.GetExtension(file).ToLower().Equals(".vbproj");
        }

        #endregion
    }
}
