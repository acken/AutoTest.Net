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
using System.Diagnostics;
using System.Reflection;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;

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

        public bool CanHandleTestFor(Project project)
        {
            if (!_configuration.UseAutoTestTestRunner)
                return false;
            return project.Value.ContainsNUnitTests || project.Value.ContainsXUnitTests;
        }

        public bool CanHandleTestFor(string assembly)
        {
            var references = _referenceResolver.GetReferences(assembly);
            return references.Contains("nunit.framework") || references.Contains("xunit");
        }

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
            var options = generateOptions(runInfos);
            if (options == null)
                return new TestRunResults[] { };
            var runner = new TestRunProcess(new AutoTestRunnerFeedback());
            var tests = runner.ProcessTestRuns(options);
            return getResults(tests, runInfos).ToArray();
        }

        private TestRunResults[] getResults(IEnumerable<AutoTest.TestRunners.Shared.Results.TestResult> tests, TestRunInfo[] runInfos)
        {
            var results = new List<TestRunResults>();
            foreach (var byRunner in tests.GroupBy(x => x.Runner))
            {
                var runner = TestRunnerConverter.FromString(byRunner.Key);
                foreach (var byAssembly in byRunner.GroupBy(x => x.Assembly))
                {
                    var info = runInfos.Where(x => x.Assembly.Equals(byAssembly.Key)).FirstOrDefault();
                    var project = "";
                    var partial = false;
                    if (info != null)
                    {
                        if (info.Project != null)
                            project = info.Project.Key;
                        partial = info.OnlyRunSpcifiedTestsFor(runner) ||
                                  info.GetTestsFor(runner).Count() > 0 ||
                                  info.GetMembersFor(runner).Count() > 0 ||
                                  info.GetNamespacesFor(runner).Count() > 0;
                    }
                    DebugLog.Debug.WriteDetail(string.Format("Partial run is {0} for runner {1}", partial, runner));
                    
                    var result = new TestRunResults(
                                        project,
                                        byAssembly.Key,
                                        partial,
                                        runner,
                                        byAssembly.Select(x => new Messages.TestResult(
                                            runner,
                                            getTestState(x.State),
                                            x.TestName,
                                            x.Message,
                                            x.StackLines.Select(y => (IStackLine)new StackLineMessage(y.Method, y.File, y.Line)).ToArray<IStackLine>()
                                            )).ToArray()
                                            );
                    result.SetTimeSpent(TimeSpan.FromMilliseconds(byAssembly.Sum(x => x.DurationInMilliseconds)));
                    results.Add(result);
                }
            }
            return results.ToArray();
        }

        private TestRunStatus getTestState(TestState testState)
        {
            switch (testState)
            {
                case TestState.Failed:
                case TestState.Panic:
                    return TestRunStatus.Failed;
                case TestState.Ignored:
                    return TestRunStatus.Ignored;
                case TestState.Passed:
                    return TestRunStatus.Passed;
            }
            return TestRunStatus.Failed;
        }

        private RunOptions generateOptions(TestRunInfo[] runInfos)
        {
            var options = new RunOptions();
            addNUnitTests(runInfos, options);
            addXUnitTests(runInfos, options);
            if (options.TestRuns.Count() == 0)
                return null;
            return options;
        }

        private void addNUnitTests(TestRunInfo[] runInfos, RunOptions options)
        {
            var nunitInfos = runInfos.Where(x => canHandle(x, "nunit.framework", (ProjectDocument doc) => { return doc.ContainsNUnitTests; }));
            if (nunitInfos.Count() > 0)
            {
                var runner = getRunnerOptions(nunitInfos, "NUnit", TestRunner.NUnit, getFramework, (TestRunInfo info) => { return _configuration.NunitTestRunner(getFramework(info)).Length > 0; });
                if (runner.Assemblies.Count() > 0)
                    options.AddTestRun(runner);
            }
        }

        private void addXUnitTests(TestRunInfo[] runInfos, RunOptions options)
        {
            var xunitInfos = runInfos.Where(x => canHandle(x, "xunit", (ProjectDocument doc) => { return doc.ContainsXUnitTests; }));
            if (xunitInfos.Count() > 0)
            {
                var runner = getRunnerOptions(xunitInfos, "XUnit", TestRunner.XUnit, getFramework, (TestRunInfo info) => { return _configuration.XunitTestRunner(getFramework(info)).Length > 0; });
                if (runner.Assemblies.Count() > 0)
                    options.AddTestRun(runner);
            }
        }

        private RunnerOptions getRunnerOptions(IEnumerable<TestRunInfo> unitInfos, string id, TestRunner testRunner, Func<TestRunInfo, string> frameworkEvaluator, Func<TestRunInfo, bool> hasOtherRunners)
        {
			DebugLog.Debug.WriteDetail("Getting runner options for {0}", id);
            var runner = new RunnerOptions(id);
            foreach (var info in unitInfos)
            {
				DebugLog.Debug.WriteDetail("Handling {0}", info.Assembly);
                if (hasOtherRunners.Invoke(info))
                    continue;
				DebugLog.Debug.WriteDetail("About to add assembly");
                var assembly = new AssemblyOptions(info.Assembly, frameworkEvaluator.Invoke(info).Replace("v", ""));
				DebugLog.Debug.WriteDetail("About to add tests");
                assembly.AddTests(info.GetTestsFor(testRunner));
                assembly.AddTests(info.GetTestsFor(TestRunner.Any));
				DebugLog.Debug.WriteDetail("About to add members");
                assembly.AddMembers(info.GetMembersFor(testRunner));
                assembly.AddMembers(info.GetMembersFor(TestRunner.Any));
				DebugLog.Debug.WriteDetail("About to add namespaces");
                assembly.AddNamespaces(info.GetNamespacesFor(testRunner));
                assembly.AddNamespaces(info.GetNamespacesFor(TestRunner.Any));
				DebugLog.Debug.WriteDetail("Should we add test run?");
                if (info.OnlyRunSpcifiedTestsFor(testRunner) && assembly.Tests.Count() == 0 && assembly.Members.Count() == 0 && assembly.Namespaces.Count() == 0)
                    continue;
				DebugLog.Debug.WriteDetail("Adding assembly");
                runner.AddAssembly(assembly);
            }
            return runner;
        }

        private string getFramework(TestRunInfo info)
        {
            if (Environment.OSVersion.Platform.Equals(PlatformID.Unix) || Environment.OSVersion.Platform.Equals(PlatformID.MacOSX))
                return "";

            if (info.Project == null || info.Project.Value == null)
                return "";
            if (info.Project.Value.Framework == null)
                return "";
            return info.Project.Value.Framework;
        }

        private bool canHandle(TestRunInfo info, string reference, Func<ProjectDocument, bool> frameworkValidator)
        {
            if (info.Project != null && info.Project.Value != null)
                return frameworkValidator.Invoke(info.Project.Value);
            return _referenceResolver.GetReferences(info.Assembly).Contains(reference);
        }
    }

    class AutoTestRunnerFeedback : ITestRunProcessFeedback
    {
        public void ProcessStart(string commandline)
        {
            DebugLog.Debug.WriteInfo("Running tests: " + commandline);
        }
    }
}
