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
        private FakeFileSystemService _fs;

        [SetUp]
        public void SetUp()
        {
            _fs = new FakeFileSystemService();
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
        public void Should_Find_CSharp_Project_With_XUnit_Tests()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.ContainsXUnitTests.ShouldBeTrue();
        }

        [Test]
        public void Should_Find_VisualBasic_Project_With_XUnit_Tests()
        {
            var document = _parser.Parse(getVisualBasicProject(), null);
            document.ContainsXUnitTests.ShouldBeTrue();
        }

        [Test]
        public void Should_find_CSharp_references()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.References.Length.ShouldEqual(1);
            document.References[0].ShouldEqual(
                Path.GetFullPath(
                    string.Format("TestResources{0}CSharpClassLibrary{0}CSharpClassLibrary.csproj",
                                  Path.DirectorySeparatorChar)
                        .Replace("\\", Path.DirectorySeparatorChar.ToString())));
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

        [Test]
        public void Should_set_assembly_name()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.AssemblyName.ShouldEqual("CSharpNUnitTestProject.dll");

            document = _parser.Parse(getVisualBasicProject(), null);
            document.AssemblyName.ShouldEqual("NUnitTestProjectVisualBasic.exe");
        }

        [Test]
        public void Should_set_build_configuration()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.BuildConfiguration.ShouldEqual("Debug");

            document = _parser.Parse(getVisualBasicProject(), null);
            document.BuildConfiguration.ShouldEqual("Debug");
        }

        [Test]
        public void Should_set_build_platform()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.Platform.ShouldEqual("AnyCPU");

            document = _parser.Parse(getVisualBasicProject(), null);
            document.Platform.ShouldEqual("AnyCPU");
        }

        [Test]
        public void Should_force_output_path_to_out_own_custom()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.OutputPath.ShouldEqual(string.Format("bin{0}AutoTest.NET{0}", Path.DirectorySeparatorChar));

            document = _parser.Parse(getVisualBasicProject(), null);
            document.OutputPath.ShouldEqual(string.Format("bin{0}AutoTest.NET{0}", Path.DirectorySeparatorChar));
        }

        [Test]
        public void Should_set_framework_version()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.Framework.Equals("v3.5");

            document = _parser.Parse(getVisualBasicProject(), null);
            document.Framework.Equals("v3.5");
        }

        [Test]
        public void Should_set_product_version()
        {
            var document = _parser.Parse(getCSharpProject(), null);
            document.ProductVersion.Equals("9.0.30729");

            document = _parser.Parse(getVisualBasicProject(), null);
            document.ProductVersion.Equals("9.0.30729");
        }
		
		[Test]
        public void Should_set_build_platform_to_x86()
        {
            var document = _parser.Parse(getCSharpNoAnyCPUProject(), null);
            document.Platform.ShouldEqual("x86");
        }

        private string getCSharpProject()
        {
            return string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar);
        }

        private string getVisualBasicProject()
        {
            return string.Format("TestResources{0}VS2008{0}NUnitTestProjectVisualBasic.vbproj", Path.DirectorySeparatorChar);
        }
		
		private string getCSharpNoAnyCPUProject()
		{
			return string.Format("TestResources{0}VS2008{0}CSharpNoAnyCPU.csproj", Path.DirectorySeparatorChar);
		}
    }
}
