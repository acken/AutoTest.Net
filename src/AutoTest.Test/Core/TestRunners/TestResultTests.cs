using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;
using NUnit.Framework;
using NUnit.Framework.Extensions;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class TestResultTests
    {
        [RowTest]
        [Row(TestStatus.Passed)]
        [Row(TestStatus.Failed)]
        [Row(TestStatus.Ignored)]
        public void Should_map_status(TestStatus status)
        {
            new TestResult(status,String.Empty).Status.ShouldEqual(status);
        }


        [Test]
        public void Should_return_passed_message()
        {
            var passedResult = TestResult.Pass();
            passedResult.Message.ShouldEqual(string.Empty);
            passedResult.Status.ShouldEqual(TestStatus.Passed);
        }

        [Test]
        public void Should_return_fail_message()
        {
            var failedMessage = TestResult.Fail("omg!");
            failedMessage.Status.ShouldEqual(TestStatus.Failed);
            failedMessage.Name.ShouldEqual("omg!");
        }

        [Test]
        public void Should_map_message()
        {
            new TestResult(TestStatus.Failed, "asdf").Name.ShouldEqual("asdf");
        }

        [Test]
        public void Should_be_equal()
        {
            var result1 = new TestResult(TestStatus.Failed, "test name", "test message", new IStackLine[] { });
            var result2 = new TestResult(TestStatus.Failed, "test name", "test message", new IStackLine[] { });
            result1.Equals(result2).ShouldBeTrue();
        }

        [Test]
        public void Should_not_be_equal()
        {
            var result1 = new TestResult(TestStatus.Failed, "test name", "test message", new IStackLine[] { });
            var result2 = new TestResult(TestStatus.Failed, "another test name", "test message", new IStackLine[] { });
            result1.Equals(result2).ShouldBeFalse();
        }
    }
}
