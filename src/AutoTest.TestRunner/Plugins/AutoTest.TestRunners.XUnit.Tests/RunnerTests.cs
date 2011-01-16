using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Options;
using System.IO;

namespace AutoTest.TestRunners.XUnit.Tests
{
    [TestFixture]
    public class RunnerTests
    {
        [Test]
        public void Should_run_tests()
        {
            var options = new RunnerOptions("XUnit");
            options.AddAssembly(new AssemblyOptions(
                Path.GetFullPath(@"AutoTest.TestRunners.XUnit.Tests.TestResource.dll")));

            var runner = new Runner();
            runner.Run(options);
        }
    }
}
