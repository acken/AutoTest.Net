using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.VS.Util.SourceParsers;
using System.IO;
using System.Reflection;

namespace AutoTest.VS.Util.Tests.SourceParsers
{
    [TestFixture]
    public class SpecFlowFeatureParserTests
    {
        [Test]
        public void when_given_something_that_is_not_a_spec_it_will_return_null()
        {
            Assert.That(new SpecFlowFeatureParser("", null).GetTest(0), Is.Null);
        }

        [Test]
        public void When_given_a_spec_flow_feature_file_it_will_find_closest_scenario()
        {
            var line = 28;
            var parser = new SpecFlowFeatureParser(getFeature(), getCodeBehind());
            var signature = parser.GetTest(line);
            Assert.That(signature.Name, Is.EqualTo("Specifications.Features.SMSPaymentsFeature.SendMoneyFromUnregisteredUser"));
            Assert.That(signature.Type, Is.EqualTo(SignatureType.Method));
        }

        [Test]
        public void When_given_a_spec_flow_feature_file_and_its_an_empty_line_execute_all_specs_in_file()
        {
            var line = 20;
            var parser = new SpecFlowFeatureParser(getFeature(), getCodeBehind());
            var signature = parser.GetTest(line);
            Assert.That(signature.Name, Is.EqualTo("Specifications.Features.SMSPaymentsFeature"));
            Assert.That(signature.Type, Is.EqualTo(SignatureType.Class));
        }

        private string getFeature()
        {
            return File.ReadAllText(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources"), "SpecflowFeature.txt"));
        }

        private string getCodeBehind()
        {
            return File.ReadAllText(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources"), "SpecflowCodeBehind.txt"));
        }
    }
}
