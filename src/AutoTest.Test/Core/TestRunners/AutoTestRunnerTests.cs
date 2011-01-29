using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.Caching.Projects;
using System.IO;
using Rhino.Mocks;
using AutoTest.Core.Configuration;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class AutoTestRunnerTests
    {
        [Test]
        public void Should_handle_nunit()
        {
            var configuration = MockRepository.GenerateMock<IConfiguration>();
            configuration.Stub(x => x.UseAutoTestTestRunner).Return(true);

            var file = string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar);
            var runner = new AutoTestTestRunner(null, configuration);
            Assert.That(runner.CanHandleTestFor(new Project(file, null)), Is.True);
        }
    }
}
