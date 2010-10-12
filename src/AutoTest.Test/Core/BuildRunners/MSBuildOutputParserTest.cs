using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.BuildRunners;
using System.IO;

namespace AutoTest.Test.Core.BuildRunners
{
    [TestFixture]
    public class MSBuildOutputParserTest
    {
        [Test]
        public void Should_parse_errors()
        {
			var resultfile = string.Format("TestResources{0}MSBuild{0}msbuild_errors.txt", Path.DirectorySeparatorChar);
            var result = new BuildRunResults("");
            var parser = new MSBuildOutputParser(result, File.ReadAllLines(resultfile));
            parser.Parse();
            result.ErrorCount.ShouldEqual(1);
            result.Errors[0].File.ShouldEqual("/home/ack/src/AutoTest.Net/src/AutoTest.Core/Messaging/MessageConsumers/ProjectChangeConsumer.cs");
            result.Errors[0].LineNumber.ShouldEqual(62);
            result.Errors[0].LinePosition.ShouldEqual(50);
            result.Errors[0].ErrorMessage.ShouldEqual("CS1003: ; expected");
        }

        [Test]
        public void Should_parse_warning()
        {
			var resultfile = string.Format("TestResources{0}MSBuild{0}msbuild_warnings.txt", Path.DirectorySeparatorChar);
            var result = new BuildRunResults("");
            var parser = new MSBuildOutputParser(result, File.ReadAllLines(resultfile));
            parser.Parse();
            result.WarningCount.ShouldEqual(2);
			
            result.Warnings[0].File.ShouldEqual("/home/ack/src/AutoTest.Net/src/AutoTest.Core/BuildRunners/MSBuildOutputParser.cs");
            result.Warnings[0].LineNumber.ShouldEqual(21);
            result.Warnings[0].LinePosition.ShouldEqual(29);
            result.Warnings[0].ErrorMessage.ShouldEqual("CS1717: Assignment made to same variable; did you mean to assign something else?");
			
			result.Warnings[1].File.ShouldEqual("/home/ack/src/AutoTest.Net/src/AutoTest.Test/Core/BuildRunners/MSBuildOutputParserTest.cs");
            result.Warnings[1].LineNumber.ShouldEqual(27);
            result.Warnings[1].LinePosition.ShouldEqual(29);
            result.Warnings[1].ErrorMessage.ShouldEqual("CS1717: Assignment made to same variable; did you mean to assign something else?");
        }
		
		[Test]
        public void Should_parse_succeeded()
        {
			var resultfile = string.Format("TestResources{0}MSBuild{0}msbuild_succeeded.txt", Path.DirectorySeparatorChar);
            var result = new BuildRunResults("");
            var parser = new MSBuildOutputParser(result, File.ReadAllLines(resultfile));
            parser.Parse();
            result.WarningCount.ShouldEqual(0);
            result.ErrorCount.ShouldEqual(0);
        }
    }
}
