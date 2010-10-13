using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using AutoTest.Core.TestRunners;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class NUnitStackLineTest
    {
        private NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line = new NUnitStackLine("at CSharpNUnitTestProject.Class1.Test1() in c:\\CSharpNUnitTestProject\\Class1.cs:line 16");
        }

        [Test]
        public void Should_parse_description_line()
        {
            var line = new NUnitStackLine("some description");
            line.ToString().ShouldEqual("some description");
            line.Method.ShouldEqual("");
            line.File.ShouldEqual("");
            line.LineNumber.ShouldEqual(0);
        }

        [Test]
        public void Should_parse_line_with_method()
        {
            _line.Method.ShouldEqual("CSharpNUnitTestProject.Class1.Test1()");
        }

        [Test]
        public void When_no_method_closing_should_return_empty_string()
        {
            var line = new NUnitStackLine("at SomeMethodWithoutParentheses in ...");
            line.Method.ShouldEqual("");
        }

        [Test]
        public void Should_parse_line_with_file()
        {
            _line.File.ShouldEqual("c:\\CSharpNUnitTestProject\\Class1.cs");
        }

        [Test]
        public void When_no_line_number_should_not_return_file()
        {
            var line = new NUnitStackLine("() in c:\\CSharpNUnitTestProject\\Class1.cs");
            line.File.ShouldEqual("");
        }

        [Test]
        public void Should_parse_line_number()
        {
            _line.LineNumber.ShouldEqual(16);
        }

        [Test]
        public void When_invalid_line_number_should_return_0()
        {
            var line = new NUnitStackLine("() in c:\\CSharpNUnitTestProject\\Class1.cs:line invalid");
            line.LineNumber.ShouldEqual(0);
        }

        [Test]
        public void When_no_line_number_should_return_0()
        {
            var line = new NUnitStackLine("() in c:\\CSharpNUnitTestProject\\Class1.cs:line");
            line.LineNumber.ShouldEqual(0);
        }
		
		[Test]
		public void Should_parse_mono_type_stack_line()
		{
			var line = new NUnitStackLine("at AutoTest.Test.Core.Configuration.DIContainerFullTest.Should_perform_full_bootstrap () [0x00018] in /home/ack/src/AutoTest.Net/src/AutoTest.Test/Core/Configuration/DIContainerFullTest.cs:20");
            line.File
                .Replace('/', Path.DirectorySeparatorChar)
                .ShouldEqual("/home/ack/src/AutoTest.Net/src/AutoTest.Test/Core/Configuration/DIContainerFullTest.cs".Replace('/', Path.DirectorySeparatorChar));
			line.LineNumber.ShouldEqual(20);
		}
    }
}
