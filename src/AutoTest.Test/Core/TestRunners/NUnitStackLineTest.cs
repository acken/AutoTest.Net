using AutoTest.Core.TestRunners;

using NUnit.Framework;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class When_parsing_a_Windows_stack_line_with_method_and_file_and_line_number
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line =
                new NUnitStackLine(
                    "at CSharpNUnitTestProject.Class1.Test1() in c:\\CSharpNUnitTestProject\\Class1.cs:line 16");
        }

        [Test]
        public void Should_parse_line_number()
        {
            _line.LineNumber.ShouldEqual(16);
        }

        [Test]
        public void Should_parse_the_file_name()
        {
            _line.File.ShouldEqual("c:\\CSharpNUnitTestProject\\Class1.cs");
        }

        [Test]
        public void Should_parse_the_method()
        {
            _line.Method.ShouldEqual("CSharpNUnitTestProject.Class1.Test1()");
        }
    }

    [TestFixture]
    public class When_parsing_a_Windows_stack_line_with_a_file_name_with_whitespace_characters
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line =
                new NUnitStackLine(
                    "at CSharpNUnitTestProject.Class1.Test1() in c:\\CSharp NUnit Test Project\\Some Class.cs:line 16");
        }

        [Test]
        public void Should_parse_the_method()
        {
            _line.Method.ShouldEqual("CSharpNUnitTestProject.Class1.Test1()");
        }

        [Test]
        public void Should_parse_the_file_name()
        {
            _line.File.ShouldEqual("c:\\CSharp NUnit Test Project\\Some Class.cs");
        }

        [Test]
        public void Should_parse_the_line_number()
        {
            _line.LineNumber.ShouldEqual(16);
        }
    }

    [TestFixture]
    public class When_parsing_a_Mono_stack_line_with_method_and_file_and_line_number
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line =
                new NUnitStackLine("at AutoTest.TestingExtensionMethods.ShouldEqual[Int32] (Int32 actual, Int32 expected) [0x00000] in /some/folder/the.file.cs:20");
        }

        [Test]
        public void Should_parse_line_number()
        {
            _line.LineNumber.ShouldEqual(20);
        }

        [Test]
        public void Should_parse_the_file_name()
        {
            _line.File.ShouldEqual("/some/folder/the.file.cs");
        }

        [Test]
        public void Should_parse_the_method()
        {
            _line.Method.ShouldEqual("AutoTest.TestingExtensionMethods.ShouldEqual[Int32] (Int32 actual, Int32 expected)");
        }
    }
    
    [TestFixture]
    public class When_parsing_a_Mono_stack_line_with_method_wothout_parameters_and_file_and_line_number
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line =
                new NUnitStackLine("at AutoTest.TestingExtensionMethods.ShouldEqual[Int32] () [0x00000] in /some/folder/the.file.cs:20");
        }

        [Test]
        public void Should_parse_line_number()
        {
            _line.LineNumber.ShouldEqual(20);
        }

        [Test]
        public void Should_parse_the_file_name()
        {
            _line.File.ShouldEqual("/some/folder/the.file.cs");
        }

        [Test]
        public void Should_parse_the_method()
        {
            _line.Method.ShouldEqual("AutoTest.TestingExtensionMethods.ShouldEqual[Int32] ()");
        }
    }

    [TestFixture]
    public class When_parsing_a_Mono_stack_line_with_a_file_name_with_whitespace_characters
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line =
                new NUnitStackLine(
                    "at AutoTest.TestingExtensionMethods.ShouldEqual[Int32] (Int32 actual, Int32 expected) [0x00000] in /some folder/the file.cs:20");
        }

        [Test]
        public void Should_parse_line_number()
        {
            _line.LineNumber.ShouldEqual(20);
        }

        [Test]
        public void Should_parse_the_file_name()
        {
            _line.File.ShouldEqual("/some folder/the file.cs");
        }

        [Test]
        public void Should_parse_the_method()
        {
            _line.Method.ShouldEqual("AutoTest.TestingExtensionMethods.ShouldEqual[Int32] (Int32 actual, Int32 expected)");
        }
    }

    [TestFixture]
    public class When_parsing_a_stack_line_with_invalid_line_number
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line = new NUnitStackLine("() in c:\\CSharpNUnitTestProject\\Class1.cs:line invalid");
        }

        [Test]
        public void Should_not_parse_the_line_number()
        {
            _line.LineNumber.ShouldEqual(0);
        }
    }

    [TestFixture]
    public class When_parsing_a_stack_line_without_line_number
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line = new NUnitStackLine("() in c:\\CSharpNUnitTestProject\\Class1.cs");
        }

        [Test]
        public void Should_not_parse_the_line_number()
        {
            _line.LineNumber.ShouldEqual(0);
        }

        [Test]
        public void Should_not_parse_the_file_name()
        {
            _line.File.ShouldEqual("");
        }
    }

    [TestFixture]
    public class When_parsing_a_stack_line_with_incomplete_line_number
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line = new NUnitStackLine("() in c:\\CSharpNUnitTestProject\\Class1.cs:line");
        }

        [Test]
        public void Should_not_parse_the_line_number()
        {
            _line.LineNumber.ShouldEqual(0);
        }

        [Test]
        public void Should_parse_the_file_name()
        {
            _line.File.ShouldEqual("c:\\CSharpNUnitTestProject\\Class1.cs");
        }
    }

    [TestFixture]
    public class When_parsing_a_stack_line_that_is_missing_method_parenthesis
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line = new NUnitStackLine("at SomeMethodWithoutParentheses in ...");
        }

        [Test]
        public void Should_not_parse_the_line_number()
        {
            _line.LineNumber.ShouldEqual(0);
        }

        [Test]
        public void Should_not_parse_the_file_name()
        {
            _line.File.ShouldEqual("");
        }
    }

    [TestFixture]
    public class When_parsing_a_stack_line_with_description
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line = new NUnitStackLine("some description");
        }

        [Test]
        public void Should_parse_description_line()
        {
            _line.ToString().ShouldEqual("some description");
            _line.Method.ShouldEqual("");
            _line.File.ShouldEqual("");
            _line.LineNumber.ShouldEqual(0);
        }

        [Test]
        public void Should_not_parse_the_method()
        {
            _line.Method.ShouldEqual("");
        }

        [Test]
        public void Should_not_parse_the_file_name()
        {
            _line.File.ShouldEqual("");
        }

        [Test]
        public void Should_not_parse_the_line_number()
        {
            _line.LineNumber.ShouldEqual(0);
        }
    }

    [TestFixture]
    public class When_parsing_a_German_stack_line
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line =
                new NUnitStackLine(
                    "bei CSharpNUnitTestProject.Class1.Test1() in c:\\CSharpNUnitTestProject\\Class1.cs:Zeile 16");
        }

        [Test]
        public void Should_parse_the_method()
        {
            _line.Method.ShouldEqual("CSharpNUnitTestProject.Class1.Test1()");
        }

        [Test]
        public void Should_parse_the_file_name()
        {
            _line.File.ShouldEqual("c:\\CSharpNUnitTestProject\\Class1.cs");
        }

        [Test]
        public void Should_parse_the_line_number()
        {
            _line.LineNumber.ShouldEqual(16);
        }
    }

    [TestFixture]
    public class When_parsing_a_Klingon_stack_line
    {
        NUnitStackLine _line;

        [SetUp]
        public void SetUp()
        {
            _line =
                new NUnitStackLine(
                    "Daq tach CSharpNUnitTestProject.Class1.Test1() daq tach c:\\CSharpNUnitTestProject\\Class1.cs:tlh kegh 16");
        }

        [Test]
        public void Should_parse_the_method()
        {
            _line.Method.ShouldEqual("CSharpNUnitTestProject.Class1.Test1()");
        }

        [Test]
        public void Should_parse_the_file_name()
        {
            _line.File.ShouldEqual("c:\\CSharpNUnitTestProject\\Class1.cs");
        }

        [Test]
        public void Should_parse_the_line_number()
        {
            _line.LineNumber.ShouldEqual(16);
        }
    }
}