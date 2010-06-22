using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;
using NUnit.Framework;
using AutoTest.WinForms.ResultsCache;

namespace AutoTest.WinForms.Test.ResultsCache
{
    [TestFixture]
    public class TestItemTest
    {
        [Test]
        public void Should_mark_stack_trace_links()
        {
            var traceBuilder = new StringBuilder();
            traceBuilder.AppendLine("at AutoTest.TestingExtensionMethods.ShouldEqual<T>(T actual, T expected) in TestingExtensionMethods.cs: line 14");
            traceBuilder.AppendLine("at AutoTest.WinForms.Test.ResultsCache.TestItemTest.Should_mark_stack_trace_links() in TestItemTest.cs: line 21");
            var result = new TestResult(TestStatus.Failed, "", "", traceBuilder.ToString());
            var item = new TestItem("", "", result);

            var itemString = new StringBuilder();
            itemString.AppendLine("Assembly: ");
            itemString.AppendLine("Test: ");
            itemString.AppendLine("Message:");
            itemString.AppendLine("");
            itemString.AppendLine("Stack trace");
            itemString.AppendLine("at AutoTest.TestingExtensionMethods.ShouldEqual<T>(T actual, T expected) in <<Link>>TestingExtensionMethods.cs: line 14<</Link>>");
            itemString.AppendLine("at AutoTest.WinForms.Test.ResultsCache.TestItemTest.Should_mark_stack_trace_links() in <<Link>>TestItemTest.cs: line 21<</Link>>");
            item.ToString().ShouldEqual(itemString.ToString());
        }
    }
}
