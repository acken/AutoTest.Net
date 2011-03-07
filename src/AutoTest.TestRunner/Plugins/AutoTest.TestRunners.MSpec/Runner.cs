using System;
using System.Collections.Generic;
using System.Linq;

using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;

namespace AutoTest.TestRunners.MSpec
{
    public class Runner : IAutoTestNetTestRunner
    {
        ILogger _logger = new NullLogger();

        public string Identifier
        {
            get { return "Machine.Specifications"; }
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsTest(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly, member);
            var field = locator.Locate();

            if (field.Category != TypeCategory.Field)
            {
                _logger.Write("{0} is not a field and therefore not a specification",
                              field.Fullname);
                return false;
            }

            var isSpecification = field.TypeName.Equals("Machine.Specifications.It");

            _logger.Write("{0} is{1} a field of type It",
                          field.Fullname,
                          isSpecification ? "" : " not");

            return isSpecification;
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly, member);
            var clazz = locator.Locate();

            if (!clazz.IsPublic)
            {
                _logger.Write("{0} is not public and therefore not a context",
                              clazz.Fullname);

                return false;
            }

            var isContext = clazz.Fields.Any(x => x.TypeName.Equals("Machine.Specifications.It"));

            _logger.Write("{0} {1} fields of type It",
                          clazz.Fullname,
                          isContext ? "contains" : "does not contain");
            return isContext;
        }

        public bool ContainsTestsFor(string assembly)
        {
            var parser = new AssemblyReader();
            var isContextContainer = parser.GetReferences(assembly).Contains("Machine.Specifications");

            _logger.Write("{0} does{1} reference Machine.Specifications",
                          assembly,
                          isContextContainer ? "" : " not");

            return isContextContainer;
        }

        public bool Handles(string identifier)
        {
            return Identifier.Equals(identifier, StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}