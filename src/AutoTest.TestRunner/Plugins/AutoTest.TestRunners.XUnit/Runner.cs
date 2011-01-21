using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Logging;

namespace AutoTest.TestRunners.XUnit
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
            return identifier.Equals("XUnit");
        }

        public IEnumerable<TestResult> Run(RunnerOptions options)
        {
            var runner = new XUnitRunner();
            return runner.Run(options);
        }
    }
}
