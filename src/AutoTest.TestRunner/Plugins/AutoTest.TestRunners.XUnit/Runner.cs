using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.XUnit
{
    public class Runner : IAutoTestNetTestRunner
    {
        public string Identifier { get { return "XUnit"; } }

        public void SetLogger(ILogger logger)
        {
        }

        public bool IsTest(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly, member);
            var method = locator.Locate();
            if (method.Category != TypeCategory.Method)
                return false;
            return method.Attributes.Contains("Xunit.FactAttribute");
        }

        public bool ContainsTests(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly, member);
            var cls = locator.Locate();
            if (cls.Category != TypeCategory.Class)
                return false;
            return cls.Methods.Where(x => x.Attributes.Contains("Xunit.FactAttribute")).Count() > 0;
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals(Identifier.ToLower());
        }

        public IEnumerable<TestResult> Run(RunnerOptions options)
        {
            var runner = new XUnitRunner();
            return runner.Run(options);
        }
    }
}
