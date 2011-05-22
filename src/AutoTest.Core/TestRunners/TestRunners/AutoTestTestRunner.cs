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
using AutoTest.Core.Caching;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Core.Messaging;

namespace AutoTest.Core.TestRunners.TestRunners
{
	public class RunLog
	{
		public string GetLocation()
		{
			var dir = Path.GetTempPath();
			var filename = string.Format("mm_output_{0}.log", Process.GetCurrentProcess().Id);
			return Path.Combine(dir, filename);
		}
	}
	
    class AutoTestTestRunner : ITestRunner
    {
        private IConfiguration _configuration;
        private ICache _cache;
        private IMessageBus _bus;
        private IRunResultCache _runCache;

        public AutoTestTestRunner(IConfiguration configuration, ICache cache, IMessageBus bus, IRunResultCache runCache)
        {
            _configuration = configuration;
            _cache = cache;
            _bus = bus;
            _runCache = runCache;
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
            if (!File.Exists(assembly))
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

        public TestRunResults[] RunTests(TestRunInfo[] runInfos, Func<bool> abortWhen)
        {
            var options = generateOptions(runInfos);
            if (options == null)
                return new TestRunResults[] { };
            var runner = new TestRunProcess(new AutoTestRunnerFeedback(_runCache, _bus))
				.ActivateProfilingFor(new RunLog().GetLocation())
				.IncludeInProfiling(getProjectReferences())
                .AbortWhen(abortWhen);
            var tests = runner.ProcessTestRuns(options);
            return getResults(tests, runInfos).ToArray();
        }
		
		private string getProjectReferences()
		{
			var references = new List<string>();
			var list = "";
            _cache.GetAll<Project>()
                .Where(x => x.Value != null).ToList()
                .ForEach(proj => references.Add(Path.GetFileNameWithoutExtension(proj.Value.AssemblyName)));
			references.ForEach(x => list += x + ", ");
            if (list.Length != 0)
                list = list.Substring(0, list.Length - 2);
            DebugLog.Debug.WriteDebug("Including in profiling: " + list);
			if (list.Length == 0)
				return null;
			return list;
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
                                        byAssembly.Select(x => ConvertResult(x)).ToArray());
                    result.SetTimeSpent(TimeSpan.FromMilliseconds(byAssembly.Sum(x => x.DurationInMilliseconds)));
                    results.Add(result);
                }
            }
            return results.ToArray();
        }

        public static Messages.TestResult ConvertResult(AutoTest.TestRunners.Shared.Results.TestResult x)
        {
            return new Messages.TestResult(TestRunnerConverter.FromString(x.Runner),
                                            getTestState(x.State),
                                            x.TestName,
                                            x.Message,
                                            x.StackLines.Select(y => (IStackLine)new StackLineMessage(y.Method, y.File, y.Line)).ToArray<IStackLine>(),
                                            x.DurationInMilliseconds
                                            );
        }

        private static TestRunStatus getTestState(TestState testState)
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
				DebugLog.Debug.WriteDetail("Handling {0}", info.Assembly);
				DebugLog.Debug.WriteDetail("About to add assembly");
                var assembly = new AssemblyOptions(info.Assembly);
                assembly.AddTests(info.GetTestsFor(testRunner));
                assembly.AddTests(info.GetTestsFor(TestRunner.Any));
                DebugLog.Debug.WriteDetail("Found {0} tests for assembly", assembly.Tests.Count());
                assembly.AddMembers(info.GetMembersFor(testRunner));
                assembly.AddMembers(info.GetMembersFor(TestRunner.Any));
                DebugLog.Debug.WriteDetail("Found {0} members for assembly", assembly.Members.Count());
                assembly.AddNamespaces(info.GetNamespacesFor(testRunner));
                assembly.AddNamespaces(info.GetNamespacesFor(TestRunner.Any));
                DebugLog.Debug.WriteDetail("Found {0} namespaces for assembly", assembly.Namespaces.Count());
                DebugLog.Debug.WriteDetail("Run only specified tests for runner {0} is {1}", testRunner, info.OnlyRunSpcifiedTestsFor(testRunner));
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
        private IRunResultCache _runCache;
        private IMessageBus _bus;

        private DateTime _lastSend = DateTime.MinValue;
        private object _padLock = new object();
        private string _currentAssembly = "";
        private int _testCount = 0;

        public AutoTestRunnerFeedback(IRunResultCache cache, IMessageBus bus)
        {
            _runCache = cache;
            _bus = bus;
        }

        public void ProcessStart(string commandline)
        {
            DebugLog.Debug.WriteInfo("Running tests: " + commandline);
        }

        public void TestFinished(AutoTest.TestRunners.Shared.Results.TestResult result)
        {
            lock (_padLock)
            {
                _currentAssembly = result.Assembly;
                _testCount++;
                if (result.State == TestState.Passed && _runCache.Failed.Count(x => x.Value.Name.Equals(result.TestName)) != 0)
                {
                    _bus.Publish(
                        new LiveTestStatusMessage(
                            _currentAssembly,
                            -1,
                            _testCount,
                            new LiveTestStatus[] { },
                            new LiveTestStatus[] { new LiveTestStatus(result.Assembly, AutoTestTestRunner.ConvertResult(result)) }));
                    _lastSend = DateTime.Now;
                    return;
                }
                else if (result.State == TestState.Failed)
                {
                    _bus.Publish(
                            new LiveTestStatusMessage(
                                _currentAssembly,
                                -1,
                                _testCount,
                                new LiveTestStatus[] { new LiveTestStatus(result.Assembly, AutoTestTestRunner.ConvertResult(result)) },
                                new LiveTestStatus[] { }));
                    _lastSend = DateTime.Now;
                    return;
                }
                else if (DateTime.Now > _lastSend.AddSeconds(1))
                {
                    _bus.Publish(
                            new LiveTestStatusMessage(
                                _currentAssembly,
                                -1,
                                _testCount,
                                new LiveTestStatus[] { },
                                new LiveTestStatus[] { }));
                    _lastSend = DateTime.Now;
                    return;
                }
            }
        }
    }
}
