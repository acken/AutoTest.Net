using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using Castle.Core.Logging;
using Rhino.Mocks;
using AutoTest.Core.Messaging;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class NUnitTestResponseParserTest
    {
        private NUnitTestResponseParser _parser;

        [SetUp]
        public void SetUp()
        {
            var bus = MockRepository.GenerateMock<IMessageBus>();
            _parser = new NUnitTestResponseParser(bus, "", "");
        }

        [Test]
        public void Should_find_succeeded_test()
        {
            _parser.Parse("<test-case name=\"CSharpNUnitTestProject.Class1.Test1\" executed=\"True\" success=\"True\" time=\"0.026\" asserts=\"1\" />");
            _parser.Result.All.Length.ShouldEqual(1);
            _parser.Result.Passed.Length.ShouldEqual(1);
            _parser.Result.Passed[0].Message.ShouldEqual("");
        }

        [Test]
        public void Should_find_test_name()
        {
            _parser.Parse("<test-case name=\"CSharpNUnitTestProject.Class1.Test1\" executed=\"True\" success=\"True\" time=\"0.026\" asserts=\"1\" />");
            _parser.Result.All[0].Name.ShouldEqual("CSharpNUnitTestProject.Class1.Test1");
        }

        [Test]
        public void Should_find_ignored_test()
        {
            _parser.Parse("<test-case name=\"CSharpNUnitTestProject.Class1.Test1\" executed=\"False\"><reason><message><![CDATA[ignored message]]></message></reason></test-case>");
            _parser.Result.All.Length.ShouldEqual(1);
            _parser.Result.Ignored.Length.ShouldEqual(1);
            _parser.Result.Ignored[0].Message.ShouldEqual("ignored message");
        }

        [Test]
        public void Should_find_failed_test()
        {
            string response = "<test-case name=\"CSharpNUnitTestProject.Class1.Test1\" executed=\"True\" " +
                              "success=\"False\" time=\"0.052\" asserts=\"1\"><failure><message><![CDATA[  " +
                              "String lengths are both 4. Strings differ at index 2. Expected: \"bleh\" But " +
                              "was:  \"blah\" -------------^]]></message><stack-trace><![CDATA[at " +
                              "CSharpNUnitTestProject.Class1.Test1() in c:\\CSharpNUnitTestProject\\Class1.cs:line 16]]>" +
                              "</stack-trace></failure></test-case>";
            _parser.Parse(response);
            _parser.Result.All.Length.ShouldEqual(1);
            _parser.Result.Failed.Length.ShouldEqual(1);
            _parser.Result.Failed[0].Message.ShouldEqual("  String lengths are both 4. Strings differ at index 2. Expected: \"bleh\" But was:  \"blah\" -------------^");
            _parser.Result.Failed[0].StackTrace.ShouldEqual("at CSharpNUnitTestProject.Class1.Test1() in c:\\CSharpNUnitTestProject\\Class1.cs:line 16");
        }
    }
}
