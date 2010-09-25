using System;
using NUnit.Framework;
using Rhino.Mocks;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.Messaging;
using System.IO;
namespace AutoTest.Test.Core.TestRunners
{
	[TestFixture]
	public class NUnitTestResponseParserMultipleAssembliesTest
	{
		private NUnitTestResponseParser _parser;

        [SetUp]
        public void SetUp()
        {
            var bus = MockRepository.GenerateMock<IMessageBus>();
            _parser = new NUnitTestResponseParser(bus, "", "");
			_parser.Parse(File.ReadAllText("TestResources/NUnit/multipleAssemblies.txt"));
        }

		[Test]
		public void Should_containt_tests_for_two_assemblies()
		{
			_parser.Result.Length.ShouldEqual(2);
		}
		
        [Test]
        public void Should_find_succeeded_test()
        {
            _parser.Result[0].All.Length.ShouldEqual(1);
            _parser.Result[1].All.Length.ShouldEqual(1);
        }
	}
}

