using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching.Projects;
using AutoTest.Messages;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Configuration;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Plugins;
using System.IO;

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
            return assemblyReferences(assembly, "nunit.framework");
        }

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
            if (!_configuration.UseAutoTestTestRunner)
                return new TestRunResults[] { };

            var optionsFile = generateOptions(runInfos);

            return new TestRunResults[] { };
        }

        private string generateOptions(TestRunInfo[] runInfos)
        {
            var plugins = new List<Plugin>();
            var options = new RunOptions();
            var nunitInfos = runInfos.Where(x => canHandleNUnit(x));
            if (nunitInfos.Count() > 0)
            {
                plugins.Add(new Plugin(Path.GetFullPath(Path.Combine("TestRunners", "AutoTest.TestRunners.NUnit.dll")), "AutoTest.TestRunners.NUnit.Runner"));
                var runner = getRunnerOptions(nunitInfos, "NUnit", TestRunner.NUnit, getFramework);
                options.AddTestRun(runner);
            }
            return "";
        }

        private RunnerOptions getRunnerOptions(IEnumerable<TestRunInfo> nunitInfos, string id, TestRunner testRunner, Func<TestRunInfo, string> frameworkEvaluator)
        {
            var runner = new RunnerOptions(id);
            foreach (var info in nunitInfos)
            {
                var assembly = new AssemblyOptions(info.Assembly, frameworkEvaluator.Invoke(info));
                assembly.AddTests(info.GetTestsFor(testRunner));
                assembly.AddTests(info.GetTestsFor(TestRunner.Any));
            }
            return runner;
        }

        private string getFramework(TestRunInfo info)
        {
            if (Environment.OSVersion.Platform.Equals(PlatformID.Unix) || Environment.OSVersion.Platform.Equals(PlatformID.MacOSX))
                return null;

            if (info.Project == null || info.Project.Value == null)
                return null;
            if (info.Project.Value.Framework == null)
                return null;
            return info.Project.Value.Framework.Replace("v", "");
        }

        private bool canHandleNUnit(TestRunInfo info)
        {
            if (info.Project != null && info.Project.Value != null)
                return info.Project.Value.ContainsNUnitTests;
            return assemblyReferences(info.Assembly, "nunit.framework");
        }

        private bool assemblyReferences(string assembly, string reference)
        {
            var references = _referenceResolver.GetReferences(assembly);
            return references.Contains(reference);
        }
    }
}
