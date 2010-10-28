using System;
using NUnit.Framework;
using AutoTest.Core.TestRunners;
namespace AutoTest.Test.Core.TestRunners
{
	[TestFixture]
	public class TestRunDetailsTest
	{
		private TestRunDetails _details;
		
		[SetUp]
		public void SetUp()
		{
			_details = new TestRunDetails(TestRunnerType.NUnit, "");
		}
		
		[Test]
		public void Should_add_single_test()
		{
			_details.AddTestToRun("MyAssembly.MyClass.MyTest");
			_details.TestsToRun.Length.ShouldEqual(1);
			_details.TestsToRun[0].ShouldEqual("MyAssembly.MyClass.MyTest");
		}
		
		[Test]
		public void Should_add_multiple_tests()
		{
			_details.AddTestsToRun(new string[] { "MyAssembly.MyClass.MyTest", "MyAssembly2.Class.AnotherTest" });
			_details.TestsToRun.Length.ShouldEqual(2);
			_details.TestsToRun[0].ShouldEqual("MyAssembly.MyClass.MyTest");
			_details.TestsToRun[1].ShouldEqual("MyAssembly2.Class.AnotherTest");
		}
	}
}

