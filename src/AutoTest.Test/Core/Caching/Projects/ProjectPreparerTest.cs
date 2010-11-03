using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;
using AutoTest.Test.Core.Caching.Projects.Fakes;
using System.IO;

namespace AutoTest.Test.Core.Caching.Projects
{
    [TestFixture]
    public class ProjectPreparerTest
    {
        private ProjectPreparer _preparer;
        private FakeCache _cache;
        private string _testProject;

        [SetUp]
        public void SetUp()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.AddReference("ReferencedProject");
            var parser = new FakeProjectParser(new ProjectDocument[] {document});
            _cache = new FakeCache();
            _preparer = new ProjectPreparer(parser, _cache);
            _testProject = Path.GetFullPath(string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar));
        }

        [Test]
        public void When_already_prepared_return_null()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.HasBeenReadFromFile();
            var record = new Project("someproject", document);
            var project = _preparer.Prepare(record);
            project.ShouldBeTheSameAs(record);
        }

        [Test]
        public void Should_prepare_project()
        {
            var record = new Project("someproject", null);
            _cache.WhenGeting("ReferencedProject")
                .Return(new Project("", new ProjectDocument(ProjectType.CSharp)));
            var project = _preparer.Prepare(record);
            project.Value.ShouldNotBeNull();
        }

        [Test]
        public void Should_Add_ReferencedProjects()
        {
            _cache.WhenGeting("ReferencedProject")
                .Return(new Project("NonExisting", new ProjectDocument(ProjectType.CSharp)));
            var record = new Project(_testProject, null);
            var project = _preparer.Prepare(record);
            project.ShouldNotBeNull();
            project.Value.References.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_populate_referenced_by()
        {
            var record = new Project("someproject", null);
            var referencedProject = new ProjectDocument(ProjectType.CSharp);
            _cache.WhenGeting("ReferencedProject")
                .Return(new Project("", referencedProject));
            var project = _preparer.Prepare(record);
            project.ShouldNotBeNull();
            referencedProject.ReferencedBy[0].ShouldEqual("someproject");
        }
    }
}
