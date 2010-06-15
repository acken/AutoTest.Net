using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;
using AutoTest.Test.Core.Caching.Projects.Fakes;
using AutoTest.Core.FileSystem;
using System.IO;

namespace AutoTest.Test.Core.Caching.Projects
{
    [TestFixture]
    public class ProjectParserTest
    {
        private ProjectParser _parser;
        private FileSystemService _fs;

        [SetUp]
        public void SetUp()
        {
            _fs = new FileSystemService();
            _parser = new ProjectParser(_fs);
        }

        [Test]
        public void Should_mark_found_project_as_read()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.IsReadFromFile.ShouldBeTrue();
        }

        [Test]
        public void Should_Find_CSharp_Project_With_NUnit_Tests()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.ContainsNUnitTests.ShouldBeTrue();
        }

        [Test]
        public void Should_Find_VisualBasic_Project_With_NUnit_Tests()
        {
            var document = _parser.Parse(getVisualBasicProject(), null);
            document.ContainsNUnitTests.ShouldBeTrue();
        }

        [Test]
        public void Should_Find_CSharp_Project_With_MS_Tests()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.ContainsMSTests.ShouldBeTrue();
        }

        [Test]
        public void Should_Find_VisualBasic_Project_With_MS_Tests()
        {
            var document = _parser.Parse(getVisualBasicProject(), null);
            document.ContainsMSTests.ShouldBeTrue();
        }

        [Test]
        public void Should_find_CSharp_references()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.References.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_find_VisualBasic_references()
        {
            var document = _parser.Parse(getVisualBasicProject(), null);
            document.References.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_add_exists_referencedby_records()
        {
            var existingDocument = new ProjectDocument(ProjectType.CSharp);
            existingDocument.AddReferencedBy("someproject");
            var document = _parser.Parse(getCSharpProject(), existingDocument);
            document.ReferencedBy[0].ShouldEqual("someproject");
        }

        private string getCSharpProject()
        {
            return string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar);
        }

        private string getVisualBasicProject()
        {
            return string.Format("TestResources{0}VS2008{0}NUnitTestProjectVisualBasic.vbproj", Path.DirectorySeparatorChar);
        }
    }
}
