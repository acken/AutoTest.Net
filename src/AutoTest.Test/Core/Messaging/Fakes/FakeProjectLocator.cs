using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem.ProjectLocators;
using AutoTest.Core.FileSystem;

namespace AutoTest.Test.Core.Messaging.Fakes
{
    class FakeProjectLocator : ILocateProjects
    {
        private ChangedFile[] _files;

        public FakeProjectLocator(ChangedFile[] files)
        {
            _files = files;
        }

        #region ILocateProjects Members

        public ChangedFile[] Locate(string file)
        {
            return _files;
        }

        #endregion
    }
}
