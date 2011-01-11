using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching.Projects;
using AutoTest.Messages;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Configuration;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class AutoTestTestRunner : ITestRunner
    {
        private IResolveAssemblyReferences _referenceResolver;
        private IConfiguration _configuration;

        public AutoTestTestRunner(IResolveAssemblyReferences referenceResolver, IConfiguration configuration)
        {
            _referenceResolver = referenceResolver;
            _configuration = configuration;
        }

        public bool CanHandleTestFor(ProjectDocument document)
        {
            return document.ContainsNUnitTests;
        }

        public bool CanHandleTestFor(string assembly)
        {
            var references = _referenceResolver.GetReferences(assembly);
            return references.Contains("nunit.framework");
        }

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
            if (!_configuration.UseAutoTestTestRunner)
                return new TestRunResults[] { };

            return new TestRunResults[] { };
        }
    }
}
