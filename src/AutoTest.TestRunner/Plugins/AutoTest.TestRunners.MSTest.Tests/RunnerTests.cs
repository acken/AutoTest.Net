using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AutoTest.TestRunners.MSTest.Tests
{
    [TestFixture]
    public class RunnerTests
    {
        [Test]
        public void Should_run_tests()
        {
            var runner = new Runner();
            runner.Run(null);
        }
    }
}
