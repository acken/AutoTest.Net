using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Logging;

namespace AutoTest.TestRunners.NUnit
{
    public class Runner : IAutoTestNetTestRunner
    {
        public void SetLogger(ILogger logger)
        {
        }

        public bool IsTest(string assembly, string type)
        {
            throw new NotImplementedException();
        }

        public bool ContainsTests(string assembly, string type)
        {
            throw new NotImplementedException();
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals("nunit");
        }

        public IEnumerable<TestResult> Run(RunnerOptions options)
        {
            var runner = new NUnitRunner();
            runner.Initialize();
            var parser = new NUnitOptionsParser(options);
            parser.Parse();
            var results = new List<TestResult>();
            foreach (var option in parser.Options)
                results.AddRange(runner.Execute(option));
            return results;
        }
    }
}
