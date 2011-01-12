using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared;
using System.IO;
using AutoTest.TestRunners.Shared.Plugins;

namespace AutoTest.TestRunners.Tests
{
    [TestFixture]
    public class OptionsXmlWriterTests
    {
        [Test]
        public void Should_produce_xml()
        {
            var plugins = new List<Plugin>();
            var options = new RunOptions();
            plugins.Add(new Plugin(@"C:\Some\Path\Assembly.dll", "This.Is.Full.Type.Name.For.Class.Implementing.IAutoTestNetTestRunner"));
            plugins.Add(new Plugin(@"C:\Some\Path\Assembly.dll", "Some.Class.Name"));

            var runner1 = new RunnerOptions("nunit");
            runner1.AddCategories(new string[] { "SomeTestCategory", "SomeOtherTestCategory" });
            var assembly1 = new AssemblyOptions(@"C:\my\testassembly.dll", "3.5");
            assembly1.AddTests(new string[] { "testassembly.class.test1", "testassembly.class.test2" });
            assembly1.AddMembers(new string[] { "testassembly.class2", "testassembly.class3" });
            assembly1.AddNamespaces(new string[] { "testassembly.somenamespace1", "testassembly.somenamespace2" });
            runner1.AddAssemblies(new AssemblyOptions[] { assembly1, new AssemblyOptions(@"C:\my\anothernunitassembly.dll", "4.0") });
            options.AddTestRun(runner1);

            var runner2 = new RunnerOptions("another");
            runner2.AddAssembly(new AssemblyOptions(@"C:\my\other\testassembly.dll"));
            options.AddTestRun(runner2);

            var writer = new OptionsXmlWriter(plugins, options);
            var file = Path.GetTempFileName();
            writer.Write(file);

            Assert.That(File.ReadAllText(file), Is.EqualTo(File.ReadAllText("TestOptions.xml")));
        }
    }
}
