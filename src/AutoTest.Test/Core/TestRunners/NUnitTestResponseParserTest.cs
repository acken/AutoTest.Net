using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using Castle.Core.Logging;
using Rhino.Mocks;
using AutoTest.Core.Messaging;
using System.IO;

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
			_parser.Parse(File.ReadAllText("TestResources/NUnit/singleAssembly.txt"));
        }

        [Test]
        public void Should_find_succeeded_test()
        {
            _parser.Result[0].All.Length.ShouldEqual(7);
            _parser.Result[0].Passed.Length.ShouldEqual(5);
            _parser.Result[0].Passed[0].Message.ShouldEqual("");
        }

        [Test]
        public void Should_find_test_name()
        {
            _parser.Result[0].All[0].Name.ShouldEqual("AutoTest.WinForms.Test.BotstrapperTest.Should_register_directoy_picker_form");
        }

        [Test]
        public void Should_find_ignored_test()
        {
            _parser.Result[0].All.Length.ShouldEqual(7);
            _parser.Result[0].Ignored.Length.ShouldEqual(1);
            _parser.Result[0].Ignored[0].Message.ShouldEqual("Ignored Test");
        }

        [Test]
        public void Should_find_failed_test()
        {
            _parser.Result[0].All.Length.ShouldEqual(7);
            _parser.Result[0].Failed.Length.ShouldEqual(1);
            _parser.Result[0].Failed[0].Message.ShouldEqual("  Expected: 10\n  But was:  2");
            _parser.Result[0].Failed[0].StackTrace.Length.ShouldEqual(4);
        }
    }
}
