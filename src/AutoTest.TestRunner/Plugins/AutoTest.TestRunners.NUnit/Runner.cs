using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.NUnit
{
    public class Runner : IAutoTestNetTestRunner
    {
        public void SetLogger(ILogger logger)
        {
        }

        public bool IsTest(string assembly, string type)
        {
            var locator = new SimpleTypeLocator(assembly, type);
            var method = locator.Locate();
            if (method.Category != TypeCategory.Method)
                return false;
            return method.Attributes.Contains("NUnit.Framework.TestAttribute") || method.Attributes.Contains("NUnit.Framework.TestCaseAttribute");
        }

        public bool ContainsTests(string assembly, string type)
        {
            var locator = new SimpleTypeLocator(assembly, type);
            var cls = locator.Locate();
            if (cls.Category != TypeCategory.Class)
                return false;
            return cls.Attributes.Contains("NUnit.Framework.TestFixtureAttribute");
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
