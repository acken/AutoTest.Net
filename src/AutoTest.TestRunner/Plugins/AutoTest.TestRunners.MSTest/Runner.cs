using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
using System.Reflection;
using System.IO;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using celer.Core.TestRunners;
using AutoTest.TestRunners.MSTest.Extensions;
using System.Diagnostics;
                                  
namespace AutoTest.TestRunners.MSTest
{
    public class Runner : IAutoTestNetTestRunner
    {
        private List<TestResult> _results;

        public string Identifier { get { return "MSTest"; } }

        public void SetLogger(ILogger logger)
        {
        }

        public bool IsTest(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly, member);
            var method = locator.Locate();
            if (method.Category != TypeCategory.Method)
                return false;
            return method.Attributes.Contains("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod");
        }

        public bool ContainsTestsFor(string assembly, string member)
        {
            var locator = new SimpleTypeLocator(assembly, member);
            var cls = locator.Locate();
            if (cls.Category != TypeCategory.Class)
                return false;
            return cls.Attributes.Contains("Microsoft.VisualStudio.TestTools.UnitTesting.TestClass");
        }

        public bool ContainsTestsFor(string assembly)
        {
            var parser = new AssemblyReader();
            return parser.GetReferences(assembly).Contains("Microsoft.VisualStudio.QualityTools.UnitTestFramework");
        }

        public bool Handles(string identifier)
        {
            return identifier.ToLower().Equals(identifier.ToLower());
        }

        public IEnumerable<TestResult> Run(RunSettings settings)
        {
            _results = new List<TestResult>();
            var tests = getTests(settings);
            var fixtures = tests.GroupBy(test => test.DeclaringType);
            foreach (var fixture in fixtures)
                runTests(settings, fixture);
            return _results;
        }

        private void runTests(RunSettings settings, IGrouping<Type, MethodInfo> fixture)
        {
            try
            {
                using (var runner = new MSTestTestRunner(fixture.Key))
                {
                    fixture.Select(test => runner.Run(test)).ToList().ForEach(x => _results.Add(getResult(settings, fixture, x)));
                }
            }
            catch (Exception ex)
            {
                _results.Add(new TestResult(Identifier, settings.Assembly.Assembly, fixture.Key.FullName, 0, "", TestState.Failed, "There was an error in class initialize or cleanup: " + Environment.NewLine + ex.Message));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
            }
        }

        private StackLine[] getStackLines(Exception ex)
        {
            if (ex == null)
                return new StackLine[] { };
            return ex.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(x => new StackLine(x)).ToArray();
        }

        private TestResult getResult(RunSettings settings, IGrouping<Type, MethodInfo> fixture, celer.Core.RunResult x)
        {
            var result = new TestResult(Identifier, settings.Assembly.Assembly, fixture.Key.FullName, x.MillisecondsSpent, fixture.Key.FullName + "." + x.Test.Name, getState(x), getMessage(x));
            result.AddStackLines(getStackLines(x.Exception));
            return result;
        }

        private string getMessage(celer.Core.RunResult x)
        {
            if (x.Exception == null)
                return "";
            return x.Exception.Message;
        }

        private TestState getState(celer.Core.RunResult x)
        {
            if (!x.WasRun)
                return TestState.Ignored;
            if (x.Passed)
                return TestState.Passed;
            else
                return TestState.Failed;
        }

        private IEnumerable<MethodInfo> getTests(RunSettings settings)
        {
            try
            {
                var assembly = Assembly.LoadFrom(settings.Assembly.Assembly);
                if (settings.Assembly.Tests.Count() > 0 || settings.Assembly.Members.Count() > 0 || settings.Assembly.Namespaces.Count() > 0)
                    return getTestSelection(assembly, settings);
                else
                    return getAllTests(assembly, settings);
            }
            catch (Exception ex)
            {
                _results.Add(new TestResult(Identifier, settings.Assembly.Assembly, "", 0, "Error while preparing runner", TestState.Panic, ex.Message));
                _results[_results.Count - 1].AddStackLines(getStackLines(ex));
                return null;
            }
        }

        private IEnumerable<MethodInfo> getTestSelection(Assembly assembly, RunSettings settings)
        {
            var tests = new List<MethodInfo>();
            assembly.GetFixtures().ForEach(fixture => tests.AddRange(fixture.GetTestsMatching(settings)));
            return tests;
        }

        private IEnumerable<MethodInfo> getAllTests(Assembly assembly, RunSettings settings)
        {
            var tests = new List<MethodInfo>();
            assembly.GetFixtures().ForEach(fixture => tests.AddRange(fixture.GetTests(settings.IgnoreCategories)));
            return tests;
        }

        private string getException(Exception ex)
        {
            var error = "";
            if (ex.InnerException != null)
                error = Environment.NewLine + getException(ex.InnerException);
            return ex.ToString() + error;
        }
    }
}
