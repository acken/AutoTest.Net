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
    public class NUnitTestRunnerTest
    {
        private NUnitTestRunner _runner;
        private IMessageBus _bus;
        private IConfiguration _configuration;
		private IResolveAssemblyReferences _referenceResolver;
        private IFileSystemService _fsService;

        [SetUp]
        public void SetUp()
        {
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _configuration = MockRepository.GenerateMock<IConfiguration>();
			_referenceResolver = MockRepository.GenerateMock<IResolveAssemblyReferences>();
            _fsService = MockRepository.GenerateMock<IFileSystemService>();

            _runner = new NUnitTestRunner(_bus, _configuration, _referenceResolver, _fsService);
        }

        [Test]
        public void Should_handle_projects_referencing_nunit()
        {
            var file = string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar);
            _configuration.Stub(c => c.NunitTestRunner("3.5")).Return("testRunner.exe");
            _fsService.Stub(x => x.FileExists("testRunner.exe")).Return(true);
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("3.5");
            _runner.CanHandleTestFor(new Project(file, document)).ShouldBeTrue();
        }

        [Test]
        public void Should_not_handle_projects_not_referencing_nunit()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            _runner.CanHandleTestFor(new Project("someProject", document)).ShouldBeFalse();
        }
		
		[Test]
		public void Should_check_for_nunit_test_framework_reference()
		{
			_referenceResolver.Stub(r => r.GetReferences("")).Return(new string[] { "nunit.framework" });
            var change = "";
            _runner.CanHandleTestFor(change).ShouldBeTrue();
		}
    }
}
