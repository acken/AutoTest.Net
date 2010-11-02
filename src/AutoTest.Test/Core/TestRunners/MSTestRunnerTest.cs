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

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class MSTestRunnerTest
    {
        private MSTestRunner _runner;
        private IConfiguration _configuration;
		private IResolveAssemblyReferences _referenceResolver;

        [SetUp]
        public void SetUp()
        {
            _configuration = MockRepository.GenerateMock<IConfiguration>();
			_referenceResolver = MockRepository.GenerateMock<IResolveAssemblyReferences>();
            _runner = new MSTestRunner(_configuration, _referenceResolver);
        }

        [Test]
        public void Should_handle_projects_referencing_mstest()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetAsMSTestContainer();
            _runner.CanHandleTestFor(document).ShouldBeTrue();
        }

        [Test]
        public void Should_not_handle_projects_not_referencing_mstest()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            _runner.CanHandleTestFor(document).ShouldBeFalse();
        }
		
		[Test]
		public void Should_check_for_ms_test_framework_reference()
		{
			_referenceResolver.Stub(r => r.GetReferences("")).Return(new string[] { "Microsoft.VisualStudio.QualityTools.UnitTestFramework" });
			var change = new ChangedFile("");
            _runner.CanHandleTestFor(change).ShouldBeTrue();
		}
    }
}
