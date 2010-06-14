using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Test.Core.Caching.Projects.Fakes
{
    class FakeProjectParser : IProjectParser
    {
        private Stack<ProjectDocument> _documents;

        public FakeProjectParser(ProjectDocument[] documents)
        {
            _documents = new Stack<ProjectDocument>(documents);
        }

        #region IProjectParser Members

        public ProjectDocument Parse(string projectFile)
        {
            return _documents.Pop();
        }

        public ProjectDocument Parse(string projectFile, ProjectDocument document)
        {
            return _documents.Pop();
        }

        #endregion
    }
}
