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
            Assemblies = @"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Test\bin\AutoTest.NET\AutoTest.Test.dll";
            Tests = "AutoTest.Test.Core.TestRunners.When_parsing_a_stack_line_with_description.Should_not_parse_the_file_name";
        }
    }
}
