using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Test.Core.Caching.Projects.Fakes;

namespace AutoTest.Test.Core.Caching
{
    [TestFixture]
    public class CacheTest
    {
        private Cache _cache;
        private ProjectDocument _firstDocument;
        private ProjectDocument _secondDocument;

        [SetUp]
        public void SetUp()
        {
            _firstDocument = new ProjectDocument(ProjectType.CSharp);
            _firstDocument.HasBeenReadFromFile();
            _secondDocument = new ProjectDocument(ProjectType.CSharp);
            _secondDocument.HasBeenReadFromFile();
            var parser = new FakeProjectParser(new ProjectDocument[] { _secondDocument, _firstDocument });
            _cache = new Cache(new FakeServiceLocator(parser, delegate { return _cache; }));
        }

        [Test]
        public void Can_Add_And_Get_Project()
        {
            _cache.Add<Project>("project");
            _cache.Get<Project>("project").ShouldNotBeNull();
        }

        [Test]
        public void Can_Get_By_Index()
        {
            _cache.Add<Project>("project");
            _cache.Get<Project>(0).ShouldNotBeNull();
        }

        [Test]
        public void Can_Check_That_Project_Exists()
        {
            _cache.Add<Project>("project");
            _cache.Exists("project").ShouldBeTrue();
        }

        [Test]
        public void Should_Not_Add_Duplicates()
        {
            _cache.Add<Project>("project");
            _cache.Add<Project>("project");
            _cache.Count.ShouldEqual(1);
        }

        [Test]
        public void Should_read_project_only_once()
        {
            _cache.Add<Project>("project");
            _cache.Get<Project>("project").Value.ShouldBeTheSameAs(_firstDocument);
        }

        [Test]
        public void When_Geting_Placeholder_It_Should_Parse_Project()
        {
            _cache.Add<Project>("project");
            _cache.Get<Project>("project").Value.ShouldNotBeNull();
        }
    }
}
