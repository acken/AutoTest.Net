using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.Messaging;
using AutoTest.Core.Configuration;
using Rhino.Mocks;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.FileSystem;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class MSTestRunnerTest
    {
        private MSTestRunner _runner;
        private IConfiguration _configuration;
		private IResolveAssemblyReferences _referenceResolver;
        private IFileSystemService _fsService;

        [SetUp]
        public void SetUp()
        {
            _configuration = MockRepository.GenerateMock<IConfiguration>();
			_referenceResolver = MockRepository.GenerateMock<IResolveAssemblyReferences>();
            _fsService = MockRepository.GenerateMock<IFileSystemService>();
            _runner = new MSTestRunner(_configuration, _referenceResolver, _fsService);
        }

        [Test]
        public void Should_handle_projects_referencing_mstest()
        {
            var projectFile = string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar);
            _configuration.Stub(c => c.MSTestRunner("3.5")).Return("testRunner.exe");
            _fsService.Stub(x => x.FileExists("testRunner.exe")).Return(true);
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("3.5");
            _runner.CanHandleTestFor(new Project(projectFile, document)).ShouldBeTrue();
        }

        [Test]
        public void Should_not_handle_projects_not_referencing_mstest()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            _runner.CanHandleTestFor(new Project("someProject", document)).ShouldBeFalse();
        }
		
		[Test]
		public void Should_check_for_ms_test_framework_reference()
		{
			_referenceResolver.Stub(r => r.GetReferences("")).Return(new string[] { "Microsoft.VisualStudio.QualityTools.UnitTestFramework" });
			var change = "";
            _runner.CanHandleTestFor(change).ShouldBeTrue();
		}
    }
}
