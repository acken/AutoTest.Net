using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching.Projects
{
    class ProjectPreparer : IPrepare<Project>
    {
        private IProjectParser _parser;
        private ICache _cache;

        public ProjectPreparer(IProjectParser parser, ICache cache)
        {
            _parser = parser;
            _cache = cache;
        }

        #region IPrepare<Project> Members

        public Project Prepare(Project record)
        {
            if (record.Value == null || !record.Value.IsReadFromFile)
                return parseProject(record);
            return record;
        }

        #endregion

        private Project parseProject(Project record)
        {
            var document = _parser.Parse(record.Key, record.Value);
            setupDependingProjects(record.Key, document);
            return new Project(record.Key, document);
        }

        private void setupDependingProjects(string key, ProjectDocument document)
        {
            foreach (var reference in document.References)
            {
                var project = _cache.Get<Project>(reference);
                
                if (project == null)
                    project = createProject(reference);

                if (!project.Value.IsReferencedBy(key))
                    project.Value.AddReferencedBy(key);
            }
        }

        private Project createProject(string reference)
        {
            _cache.Add<Project>(reference);
            return _cache.Get<Project>(reference);
        }
    }
}
