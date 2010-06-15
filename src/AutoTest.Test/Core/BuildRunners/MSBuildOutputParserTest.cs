using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.BuildRunners;

namespace AutoTest.Test.Core.BuildRunners
{
    [TestFixture]
    public class MSBuildOutputParserTest
    {
        [Test]
        public void Should_parse_error()
        {
            var result = new BuildRunResults();
            var line =
                "Class1.cs(5,7): error CS0246: The type or namespace name 'Nunit' could not be found (are you missing a using directive or an assembly reference?)";
            var parser = new MSBuildOutputParser(result, line);
            parser.Parse();
            result.ErrorCount.ShouldEqual(1);
            result.Errors[0].File.ShouldEqual("Class1.cs");
            result.Errors[0].LineNumber.ShouldEqual(5);
            result.Errors[0].LinePosition.ShouldEqual(7);
            result.Errors[0].ErrorMessage.ShouldEqual("CS0246: The type or namespace name 'Nunit' could not be found (are you missing a using directive or an assembly reference?)");
        }

        [Test]
        public void Should_parse_warning()
        {
            var result = new BuildRunResults();
            var line =
                "Session.cs(32,46): warning CS0109: The member `Desktopcouch.Session.GType' does not hide an inherited member. The new keyword is not required";
            var parser = new MSBuildOutputParser(result, line);
            parser.Parse();
            result.WarningCount.ShouldEqual(1);
            result.Warnings[0].File.ShouldEqual("Session.cs");
            result.Warnings[0].LineNumber.ShouldEqual(32);
            result.Warnings[0].LinePosition.ShouldEqual(46);
            result.Warnings[0].ErrorMessage.ShouldEqual("CS0109: The member `Desktopcouch.Session.GType' does not hide an inherited member. The new keyword is not required");
        }
    }
}
