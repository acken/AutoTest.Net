using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.XUnit
{
    public class Runner : IAutoTestNetTestRunner
    {
        private ITestFeedbackProvider _channel = null;

        public string Identifier { get { return "XUnit"; } }

        public void SetLogger(ILogger logger)
        {
        }

        public void SetLiveFeedbackChannel(ITestFeedbackProvider channel)
        {
            _channel = channel;
        }

        public bool IsTest(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly);
            var method = locator.LocateMethod(member);
            if (method == null)
                return false;
            return method.Attributes.Contains("Xunit.FactAttribute") &&
                !method.IsAbstract;
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly);
            var cls = locator.LocateClass(member);
            if (cls == null)
                return false;
            return !cls.IsAbstract && cls.Methods.Where(x => x.Attributes.Contains("Xunit.FactAttribute")).Count() > 0;
        }

        public bool ContainsTestsFor(string assembly)
        {
            var parser = new AssemblyReader();
            return parser.GetReferences(assembly).Contains("xunit");
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals(Identifier.ToLower());
        }

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            var runner = new XUnitRunner();
            return runner.Run(settings, _channel);
        }
    }
}
