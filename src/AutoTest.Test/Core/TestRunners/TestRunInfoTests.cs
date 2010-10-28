using System;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.Projects;
namespace AutoTest.Test
{
	[TestFixture]
	public class TestRunInfoTests
	{
		private TestRunInfo _info;
		
		[SetUp]
		public void SetUp()
		{
			_info = new TestRunInfo(new Project("", new ProjectDocument(ProjectType.CSharp)), "");
		}
		
		[Test]
		public void Should_add_multiple_tests()
		{
			_info.AddTestsToRun(new string[] { "MyAssembly.MyClass.MyTest", "MyAssembly2.Class.AnotherTest" });
			_info.TestsToRun.Length.ShouldEqual(2);
			_info.TestsToRun[0].ShouldEqual("MyAssembly.MyClass.MyTest");
			_info.TestsToRun[1].ShouldEqual("MyAssembly2.Class.AnotherTest");
		}
	}
}

