using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.Caching.Projects;
using Rhino.Mocks;
using AutoTest.Core.Configuration;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class XUnitTestRunnerTest
    {
        private XUnitTestRunner _runner;
        private IMessageBus _bus;
        private IConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _configuration = MockRepository.GenerateMock<IConfiguration>();
            _runner = new XUnitTestRunner(_bus, _configuration);
        }

        [Test]
        public void Should_handle_projects_referencing_xunit()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetAsXUnitTestContainer();
            _runner.CanHandleTestFor(document).ShouldBeTrue();
        }

        [Test]
        public void Should_not_handle_projects_not_referencing_xunit()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            _runner.CanHandleTestFor(document).ShouldBeFalse();
        }
    }
}
