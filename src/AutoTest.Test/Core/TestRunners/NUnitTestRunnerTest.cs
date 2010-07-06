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

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class NUnitTestRunnerTest
    {
        private NUnitTestRunner _runner;
        private IMessageBus _bus;
        private IConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _configuration = MockRepository.GenerateMock<IConfiguration>();
            _runner = new NUnitTestRunner(_bus, _configuration);
        }

        [Test]
        public void Should_handle_projects_referencing_nunit()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetAsNUnitTestContainer();
            _runner.CanHandleTestFor(document).ShouldBeTrue();
        }

        [Test]
        public void Should_not_handle_projects_not_referencing_nunit()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            _runner.CanHandleTestFor(document).ShouldBeFalse();
        }
    }
}
