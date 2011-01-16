using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Results;

namespace AutoTest.TestRunners.Tests.Results
{
    [TestFixture]
    public class ResultXmlReaderTests
    {
        [Test]
        public void Should_read_xml()
        {
            var reader = new ResultXmlReader("Results.xml");
            var results = reader.Read();
            Assert.That(results.Count(), Is.EqualTo(5));
            Assert.That(results.ElementAt(0).Assembly, Is.EqualTo(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\bin\AutoTest.NET\AutoTest.Runners.NUnit.Tests.TestResource.dll"));
            Assert.That(results.ElementAt(0).Message, Is.EqualTo("failing test"));
            Assert.That(results.ElementAt(0).State, Is.EqualTo(AutoTest.TestRunners.Shared.Results.TestState.Failed));
            Assert.That(results.ElementAt(0).TestFixture, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture1"));
            Assert.That(results.ElementAt(0).DurationInMilliseconds, Is.EqualTo(100));
            Assert.That(results.ElementAt(0).TestName, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_fail"));
            Assert.That(results.ElementAt(0).StackLines.Count(), Is.EqualTo(1));
            Assert.That(results.ElementAt(0).StackLines.ElementAt(0).File, Is.EqualTo(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\Fixture1.cs"));
            Assert.That(results.ElementAt(0).StackLines.ElementAt(0).Line, Is.EqualTo(21));
            Assert.That(results.ElementAt(0).StackLines.ElementAt(0).Method, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_fail()"));

            Assert.That(results.ElementAt(1).State, Is.EqualTo(AutoTest.TestRunners.Shared.Results.TestState.Ignored));
            Assert.That(results.ElementAt(1).StackLines.Count(), Is.EqualTo(1));

            Assert.That(results.ElementAt(4).Assembly, Is.EqualTo(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Runners.NUnit.Tests.TestResource\bin\AutoTest.NET\AutoTest.Runners.NUnit.Tests.TestResource.dll"));
            Assert.That(results.ElementAt(4).DurationInMilliseconds, Is.EqualTo(250));
            Assert.That(results.ElementAt(4).Message, Is.EqualTo(""));
            Assert.That(results.ElementAt(4).State, Is.EqualTo(AutoTest.TestRunners.Shared.Results.TestState.Passed));
            Assert.That(results.ElementAt(4).TestFixture, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture2"));
            Assert.That(results.ElementAt(4).TestName, Is.EqualTo("AutoTest.Runners.NUnit.Tests.TestResource.Fixture2.Should_also_pass_again"));
            Assert.That(results.ElementAt(4).StackLines.Count(), Is.EqualTo(0));
        }
    }
}
