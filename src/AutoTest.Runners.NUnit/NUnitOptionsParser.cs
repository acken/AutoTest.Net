using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;

namespace AutoTest.TestRunners.NUnit
{
    class NUnitOptionsParser
    {
        private RunnerOptions _options;

        public string Tests { get; private set; }
        public string Categories { get; private set; }
        public string Assemblies { get; private set; }
        public string Framework { get; private set; }

        public NUnitOptionsParser(RunnerOptions options)
        {
            _options = options;
        }

        public void Parse()
        {
            Assemblies = System.IO.Path.GetFullPath(@"AutoTest.Runners.NUnit.Tests.TestResource.dll");
            Tests = "AutoTest.Runners.NUnit.Tests.TestResource.Fixture1.Should_pass";
        }
    }
}
