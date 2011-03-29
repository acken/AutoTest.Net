using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Options;
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Runtime;
using System.IO;

namespace AutoTest.TestRunners.MbUnit
{
    public class Runner : IAutoTestNetTestRunner
    {
        private Gallio.Runtime.RuntimeSetup _setup;
        private Gallio.Runtime.Logging.ILogger _logger;
        private ITestDriver _testDriver;
        private ILogger _internalLogger;

        public Runner()
        {
            // Create a runtime setup.
            // There are a few things you can tweak here if you need.
            _setup = new Gallio.Runtime.RuntimeSetup();

            // Create a logger.
            // You can use the NullLogger but you will probably want a wrapper around your own ILogger thingy.
            _logger = Gallio.Runtime.Logging.NullLogger.Instance;

            // Initialize the runtime.
            // You only do this once.
            RuntimeBootstrap.Initialize(_setup, _logger);

            // Create a test framework selector.
            // This is used by Gallio to filter the set of frameworks it will support.
            // You can set a predicate Filter here.  I've hardcoded MbUnit here but you could leave the filter out and set it to null.
            // The fallback mode tells Gallio what to do if it does not recognize the test framework associated with the test assembly.
            // Strict means don't do anything.  You might want to use Default or Approximate.  See docs.
            // You can also set options, probably don't care.
            var testFrameworkSelector = new TestFrameworkSelector()
            {
                Filter = testFrameworkHandle => testFrameworkHandle.Id == "MbUnit.TestFramework",
                FallbackMode = TestFrameworkFallbackMode.Strict
            };

            // Now we need to get a suitably configured ITestDriver...
            var testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
            _testDriver = testFrameworkManager.GetTestDriver(testFrameworkSelector, _logger);
        }

        public string Identifier
        {
            get { throw new NotImplementedException(); }
        }

        public void SetLogger(ILogger logger)
        {
            _internalLogger = logger;
        }

        public bool IsTest(string assembly, string member)
        {
            MemberInfo mem = null; // whatever you need to do to get it.

            IMemberInfo memberInfo = Gallio.Common.Reflection.Reflector.Wrap(mem);
            IList<TestPart> testParts = _testDriver.GetTestParts(Reflector.NativeReflectionPolicy, memberInfo);
            foreach (TestPart testPart in testParts)
            {
                if (testPart.IsTest)
                    return true;
            }
            return false;
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            throw new NotImplementedException();
        }

        public bool ContainsTestsFor(string assembly)
        {
            throw new NotImplementedException();
        }

        public bool Handles(string identifier)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AutoTest.TestRunners.Shared.Results.TestResult> Run(RunSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
