using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Core.TestRunners;

namespace AutoTest.Test.Core.TestRunners
{
     [TestFixture]
    public class MSTestResponseParserTest
    {
         private MSTestResponseParser _parser;

         [SetUp]
         public void SetUp()
         {
             _parser = new MSTestResponseParser("", "");
         }

         [Test]
         public void Should_find_passed_test()
         {
             _parser.ParseLine("Passed               The_name_of_the_test");
             var result = _parser.Result;

             result.Passed.Length.ShouldEqual(1);
             result.All[0].Status.ShouldEqual(TestStatus.Passed);
             result.All[0].Message.ShouldEqual("The_name_of_the_test");
         }

         [Test]
         public void Should_find_failed_test()
         {
             _parser.ParseLine("Failed               The_name_of_the_test");
             var result = _parser.Result;

             result.Failed.Length.ShouldEqual(1);
             result.All[0].Status.ShouldEqual(TestStatus.Failed);
             result.All[0].Message.ShouldEqual("The_name_of_the_test");
         }

         [Test]
         public void Should_find_ignored_test()
         {
             _parser.ParseLine("Ignored               The_name_of_the_test");
             var result = _parser.Result;

             result.Ignored.Length.ShouldEqual(1);
             result.All[0].Status.ShouldEqual(TestStatus.Ignored);
             result.All[0].Message.ShouldEqual("The_name_of_the_test");
         }

         [Test]
         public void Should_find_inconclusive_and_treat_as_ignored()
         {
             _parser.ParseLine("Inconclusive               The_name_of_the_test");
             var result = _parser.Result;

             result.Ignored.Length.ShouldEqual(1);
             result.All[0].Status.ShouldEqual(TestStatus.Ignored);
             result.All[0].Message.ShouldEqual("The_name_of_the_test");
         }
    }
}
