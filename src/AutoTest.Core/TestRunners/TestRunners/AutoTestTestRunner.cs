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
            return CanHandleTestFor(project.GetAssembly(_configuration.CustomOutputPath));
        }

        public bool CanHandleTestFor(string assembly)
        {
            if (!_configuration.UseAutoTestTestRunner)
                return false;
            var plugins = new PluginLocator().Locate();
            foreach (var plugin in plugins)
            {
                var instance = plugin.New();
                if (instance == null)
                    continue;
                if (instance.ContainsTestsFor(assembly))
                    return true;
            }
            return false;
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
            var plugins = new PluginLocator().Locate();
            foreach (var plugin in plugins)
            {
                var testRun = getTests(plugin, runInfos);
                if (testRun != null)
                    options.AddTestRun(testRun);
            }
                
            if (options.TestRuns.Count() == 0)
                return null;
            return options;
        }

        private RunnerOptions getTests(Plugin plugin, TestRunInfo[] runInfos)
        {
            var instance = plugin.New();
            if (instance == null)
                return null;
            var infos = runInfos.Where(x => instance.ContainsTestsFor(x.Assembly));
            if (infos.Count() == 0)
                return null;
            return getRunnerOptions(infos, instance);
        }

        private RunnerOptions getRunnerOptions(IEnumerable<TestRunInfo> unitInfos, IAutoTestNetTestRunner instance)
        {
            DebugLog.Debug.WriteDetail("Getting runner options for {0}", instance.Identifier);
            var runner = new RunnerOptions(instance.Identifier);
            var testRunner = TestRunnerConverter.FromString(instance.Identifier);
            foreach (var info in unitInfos)
            {
                if (!instance.ContainsTestsFor(info.Assembly))
                    continue;
				DebugLog.Debug.WriteDetail("Handling {0}", info.Assembly);
				DebugLog.Debug.WriteDetail("About to add assembly");
                var assembly = new AssemblyOptions(info.Assembly);
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
    }

    class AutoTestRunnerFeedback : ITestRunProcessFeedback
    {
        public void ProcessStart(string commandline)
        {
            DebugLog.Debug.WriteInfo("Running tests: " + commandline);
        }
    }
}
