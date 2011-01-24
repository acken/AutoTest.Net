using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners;

namespace AutoTest.TestRunners.Tests
{
    [TestFixture]
    public class ArgumentParserTests
    {
        [Test]
        public void Should_parse_input_file()
        {
            var parser = new ArgumentParser(new string[] { "--input=\"C:\\somewhere\\something.meh\"" });
            var arguments = parser.Parse();
            Assert.That(arguments.InputFile, Is.EqualTo("C:\\somewhere\\something.meh"));
        }

        [Test]
        public void Should_parse_output_file()
        {
            var parser = new ArgumentParser(new string[] { "--output=\"C:\\somewhere\\something.meh\"" });
            var arguments = parser.Parse();
            Assert.That(arguments.OutputFile, Is.EqualTo("C:\\somewhere\\something.meh"));
        }

        [Test]
        public void Should_parse_start_suspended()
        {
            var parser = new ArgumentParser(new string[] { "--startsuspended" });
            var arguments = parser.Parse();
            Assert.That(arguments.StartSuspended, Is.True);
        }

        [Test]
        public void Should_parse_silent()
        {
            var parser = new ArgumentParser(new string[] { "--silent" });
            var arguments = parser.Parse();
            Assert.That(arguments.Silent, Is.True);
        }
    }
}
