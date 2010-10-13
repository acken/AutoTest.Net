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
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;

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
            _parser = new NUnitTestResponseParser(bus);
			var sources = new TestRunInfo[]
				{ 
					new TestRunInfo(new Project("project1", null), "/SomePath/AutoTest.WinForms.Test/bin/Debug/AutoTest.WinForms.Test.dll")
				};
			_parser.Parse(File.ReadAllText("TestResources/NUnit/singleAssembly.txt"), sources);
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
            _parser.Result[0].Failed[0].Message.ShouldEqual(string.Format("  Expected: 10{0}  But was:  2", Environment.NewLine));
            _parser.Result[0].Failed[0].StackTrace.Length.ShouldEqual(4);
			_parser.Result[0].Failed[0].StackTrace[0].File
                .Replace('/', Path.DirectorySeparatorChar)
                .ShouldEqual("/home/ack/src/AutoTest.Net/src/AutoTest.Core/Caching/RunResultCache/LinkParser.cs".Replace('/', Path.DirectorySeparatorChar));
			_parser.Result[0].Failed[0].StackTrace[1].File
                .Replace('/', Path.DirectorySeparatorChar)
                .ShouldEqual("/home/ack/src/AutoTest.Net/src/AutoTest.Test/Core/Caching/RunResultCache/LinkParserTest.cs".Replace('/', Path.DirectorySeparatorChar));
			_parser.Result[0].Failed[0].StackTrace[3].File
                .ShouldEqual("/home/ack/src/mono2.8/mono-2.8/mcs/class/corlib/System.Reflection/MonoMethod.cs".Replace('/', Path.DirectorySeparatorChar));
        }
		
		[Test]
		public void Should_extract_assemblies()
		{
			_parser.Result[0].Assembly.ShouldEqual("/SomePath/AutoTest.WinForms.Test/bin/Debug/AutoTest.WinForms.Test.dll");
		}
		
		[Test]
		public void Should_extract_run_time()
		{
			_parser.Result[0].TimeSpent.ShouldEqual(new TimeSpan(0, 0, 0, 2, 428));
		}
    }
}
