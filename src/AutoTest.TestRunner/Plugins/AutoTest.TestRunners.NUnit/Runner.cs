using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;

namespace AutoTest.TestRunners.NUnit
{
    public class Runner : IAutoTestNetTestRunner
    {
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
