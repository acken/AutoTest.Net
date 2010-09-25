using System;
using NUnit.Framework;
using Rhino.Mocks;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.Messaging;
using System.IO;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;
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
			var sources = new TestRunInfo[]
				{ 
					new TestRunInfo(new Project("project1", null), string.Format("{0}SomePath{0}AutoTest.WinForms.Test{0}bin{0}Debug{0}AutoTest.WinForms.Test.dll", Path.DirectorySeparatorChar)),
					new TestRunInfo(new Project("project2", null), string.Format("{0}SomePath{0}AutoTest.Console.Test{0}bin{0}Debug{0}AutoTest.Console.Test.dll", Path.DirectorySeparatorChar))
				};
			_parser.Parse(File.ReadAllText("TestResources/NUnit/multipleAssemblies.txt"), sources);
        }

		[Test]
		public void Should_containt_tests_for_two_assemblies()
		{
			_parser.Result.Length.ShouldEqual(2);
		}
		
		[Test]
		public void Should_extract_assemblies()
		{
			_parser.Result[0].Assembly.ShouldEqual(string.Format("{0}SomePath{0}AutoTest.WinForms.Test{0}bin{0}Debug{0}AutoTest.WinForms.Test.dll", Path.DirectorySeparatorChar));
			_parser.Result[1].Assembly.ShouldEqual(string.Format("{0}SomePath{0}AutoTest.Console.Test{0}bin{0}Debug{0}AutoTest.Console.Test.dll", Path.DirectorySeparatorChar));
		}
		
		[Test]
		public void Should_extract_run_time()
		{
			_parser.Result[0].TimeSpent.ShouldEqual(new TimeSpan(0, 0, 0, 2, 415));
			_parser.Result[1].TimeSpent.ShouldEqual(new TimeSpan(0, 0, 0, 0, 884));
		}
		
        [Test]
        public void Should_find_succeeded_test()
        {
            _parser.Result[0].All.Length.ShouldEqual(7);
            _parser.Result[1].All.Length.ShouldEqual(1);
        }
	}
}

