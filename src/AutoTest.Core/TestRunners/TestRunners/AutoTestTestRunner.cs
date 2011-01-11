using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching.Projects;
using AutoTest.Messages;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.FileSystem;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class AutoTestTestRunner : ITestRunner
    {
        private IResolveAssemblyReferences _referenceResolver;

        public AutoTestTestRunner(IResolveAssemblyReferences referenceResolver)
        {
            _referenceResolver = referenceResolver;
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
            return new TestRunResults[] { };
        }
    }
}
