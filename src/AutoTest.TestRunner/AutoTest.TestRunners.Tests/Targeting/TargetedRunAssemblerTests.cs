using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Targeting;
using Rhino.Mocks;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.Tests.Targeting
{
    [TestFixture]
    public class TargetedRunAssemblerTests
    {
        [Test]
        public void Should_group_by_assembly()
        {
            var locator = MockRepository.GenerateMock<ITargetFrameworkLocator>();
            locator.Stub(x => x.Locate("Assembly1")).Return(new Version(2, 0));
            locator.Stub(x => x.Locate("Assembly3")).Return(new Version(2, 0));
            locator.Stub(x => x.Locate("Assembly4")).Return(new Version(2, 0));
            locator.Stub(x => x.Locate("Assembly2")).Return(new Version(4, 0));

            var options = new RunOptions();
            var runner = new RunnerOptions("NUnit");
            runner.AddAssemblies(new AssemblyOptions[] { new AssemblyOptions("Assembly1"), new AssemblyOptions("Assembly2") });
            options.AddTestRun(runner);
            runner = new RunnerOptions("NUnit");
            runner.AddAssemblies(new AssemblyOptions[] { new AssemblyOptions("Assembly3") });
            options.AddTestRun(runner);
            runner = new RunnerOptions("XUnit");
            runner.AddAssemblies(new AssemblyOptions[] { new AssemblyOptions("Assembly4") });
            options.AddTestRun(runner);

            var assembler = new TargetedRunAssembler(options, locator);
            var targeted = assembler.Assemble();

            Assert.That(targeted.Count(), Is.EqualTo(2));
            Assert.That(targeted.ElementAt(0).TargetFramework, Is.EqualTo(new Version(2, 0)));
            Assert.That(targeted.ElementAt(0).Runners.Count(), Is.EqualTo(2));
            Assert.That(targeted.ElementAt(0).Runners.Count(), Is.EqualTo(2));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(0).ID, Is.EqualTo("NUnit"));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly1"));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(0).Assemblies.ElementAt(1).Assembly, Is.EqualTo("Assembly3"));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(1).ID, Is.EqualTo("XUnit"));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(1).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly4"));
            Assert.That(targeted.ElementAt(1).TargetFramework, Is.EqualTo(new Version(4, 0)));
            Assert.That(targeted.ElementAt(1).Runners.ElementAt(0).ID, Is.EqualTo("NUnit"));
            Assert.That(targeted.ElementAt(1).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly2"));
        }
    }
}
