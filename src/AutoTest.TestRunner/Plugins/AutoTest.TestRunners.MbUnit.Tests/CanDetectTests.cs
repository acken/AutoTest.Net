using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.TestRunners.MbUnit.Tests
{
    [TestFixture]
    public class When_looking_for_a_specific_test_in_an_assembly : TestRunnerScenario
    {
        [Test]
        public void and_the_test_does_not_exist_it_returns_false()
        {
            Assert.That(_runner.IsTest(getAssembly(), "AutoTest.TestRunners.MbUnitTests.Tests.TestResource.A_passing_test_that_does_not_exist"), Is.False);
        }

        [Test]
        public void and_the_test_exists_it_returns_true()
        {
            Assert.That(_runner.IsTest(getAssembly(), "AutoTest.TestRunners.MbUnitTests.Tests.TestResource.A_passing_test"), Is.True);
        }
    }
}
