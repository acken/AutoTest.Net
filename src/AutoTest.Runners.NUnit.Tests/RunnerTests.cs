using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.NUnit;

namespace AutoTest.Runners.NUnit.Tests
{
    [TestFixture]
    public class RunnerTests
    {
        [Test]
        public void Should_run_test()
        {
            var options = new RunOptions();
            options.AddRunner(new RunnerOptions());
            var runner = new NUnitRunner();
            runner.Initialize();
            runner.Execute(options.Runners.First());
        }
    }
}
