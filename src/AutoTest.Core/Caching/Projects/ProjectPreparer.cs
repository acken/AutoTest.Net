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

        public Project Prepare(Project record, Action<Project> addUnprepared)
        {
            if (record.Value == null || !record.Value.IsReadFromFile)
                return parseProject(record, addUnprepared);
            return null;
        }

        #endregion

        private Project parseProject(Project record, Action<Project> addUnprepared)
        {
            var document = _parser.Parse(record.Key, record.Value);
            setupDependingProjects(record.Key, document, addUnprepared);
            return new Project(record.Key, document);
        }

        private void setupDependingProjects(string key, ProjectDocument document, Action<Project> addUnprepared)
        {
            foreach (var reference in document.References)
            {
                var project = _cache.Get<Project>(reference);
                
                if (project == null)
                    project = createProject(reference, addUnprepared);

                if (!project.Value.IsReferencedBy(key))
                    project.Value.AddReferencedBy(key);
            }
        }

        private Project createProject(string reference, Action<Project> addUnprepared)
        {
            Project project = new Project(reference, new ProjectDocument(ProjectType.None));
            addUnprepared.Invoke(project);
            return project;
        }
    }
}
