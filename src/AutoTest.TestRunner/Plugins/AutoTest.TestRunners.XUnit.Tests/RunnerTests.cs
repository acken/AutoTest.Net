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
        private Runner _runner;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _runner = new Runner();
        }

        [Test]
        public void Should_run_tests()
        {
            var options = new RunnerOptions("XUnit");
            options.AddAssembly(new AssemblyOptions(
                Path.GetFullPath(@"AutoTest.TestRunners.XUnit.Tests.TestResource.dll")));

            var result = _runner.Run(options);

            Assert.That(result.Count(), Is.EqualTo(7));
            var test1 = result.Where(x => x.TestName.Equals("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_pass")).First();
            Assert.That(test1.Runner, Is.EqualTo("XUnit"));
            Assert.That(test1.Assembly, Is.EqualTo(options.Assemblies.ElementAt(0).Assembly));
            Assert.That(test1.TestFixture, Is.EqualTo(""));
            Assert.That(test1.DurationInMilliseconds, Is.GreaterThan(0));
            Assert.That(test1.TestName, Is.EqualTo("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_pass"));
            Assert.That(test1.State, Is.EqualTo(Shared.Results.TestState.Passed));

            var test2 = result.Where(x => x.TestName.Equals("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_fail")).First();
            Assert.That(test2.Runner, Is.EqualTo("XUnit"));
            Assert.That(test2.Assembly, Is.EqualTo(options.Assemblies.ElementAt(0).Assembly));
            Assert.That(test2.TestFixture, Is.EqualTo(""));
            Assert.That(test2.TestName, Is.EqualTo("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_fail"));
            Assert.That(test2.State, Is.EqualTo(Shared.Results.TestState.Failed));
            Assert.That(test2.StackLines.Count(), Is.EqualTo(1));
            Assert.That(test2.StackLines.ElementAt(0).Method, Is.EqualTo("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_fail()"));
            // Only verify if build with debug
            if (test2.StackLines.ElementAt(0).File.Length > 0)
            {
                Assert.IsTrue(File.Exists(test2.StackLines.ElementAt(0).File));
                Assert.That(test2.StackLines.ElementAt(0).Line, Is.EqualTo(23));
            }
        }

        [Test]
        public void Should_run_single_test()
        {
            //var options = new RunnerOptions("XUnit");
            //var assembly = new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.XUnit.Tests.TestResource.dll"));
            //assembly.AddTest("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1.Should_pass");
            //options.AddAssembly(assembly);

            //var result = _runner.Run(options);

            //Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Should_run_members()
        {
            var options = new RunnerOptions("XUnit");
            var assembly = new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.XUnit.Tests.TestResource.dll"));
            assembly.AddMember("AutoTest.TestRunners.XUnit.Tests.TestResource.Class1");
            options.AddAssembly(assembly);

            var result = _runner.Run(options);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Should_run_namespace()
        {
            //var options = new RunnerOptions("XUnit");
            //var assembly = new AssemblyOptions(Path.GetFullPath(@"AutoTest.TestRunners.XUnit.Tests.TestResource.dll"));
            //assembly.AddNamespace("AutoTest.TestRunners.XUnit.Tests.TestResource.Anothernamespace");
            //options.AddAssembly(assembly);

            //var result = _runner.Run(options);

            //Assert.That(result.Count(), Is.EqualTo(4));
        }
    }
}
