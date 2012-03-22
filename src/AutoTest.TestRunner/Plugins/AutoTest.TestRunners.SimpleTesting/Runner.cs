using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;

namespace AutoTest.TestRunners.SimpleTesting
{
    public class Runner : IAutoTestNetTestRunner
    {
        private ILogger _logger;
        private Func<string, IReflectionProvider> _reflectionProviderFactory;
        private ITestFeedbackProvider _testFeedbackProviderChannel;

        public string Identifier
        {
            get { return "SimpleTesting"; }
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void SetReflectionProvider(Func<string, IReflectionProvider> reflectionProviderFactory)
        {
            _reflectionProviderFactory = reflectionProviderFactory;
        }

        public void SetLiveFeedbackChannel(ITestFeedbackProvider channel)
        {
            _testFeedbackProviderChannel = channel;
        }

        public bool IsTest(string assembly, string member)
        {
            throw new NotImplementedException();
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
            return identifier.ToLower().Equals(Identifier.ToLower());
        }

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
