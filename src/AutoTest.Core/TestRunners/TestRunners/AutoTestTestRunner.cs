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

        public bool CanHandleTestFor(ProjectDocument document)
        {
            if (!_configuration.UseAutoTestTestRunner)
                return false;
            return document.ContainsNUnitTests || document.ContainsXUnitTests;
        }

        public bool CanHandleTestFor(string assembly)
        {
            var references = _referenceResolver.GetReferences(assembly);
            return references.Contains("nunit.framework") || references.Contains("xunit");
        }

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
            var optionsFile = Path.GetTempFileName();
            var outputFile = Path.GetTempFileName();
            if (!generateOptions(runInfos, optionsFile))
                return new TestRunResults[] { };

            DebugLog.Debug.WriteDetail(File.ReadAllText(optionsFile));
            RunTests(optionsFile, outputFile);
            DebugLog.Debug.WriteDetail(File.ReadAllText(outputFile));
            var results = getResults(outputFile, runInfos);

            if (!_configuration.DebuggingEnabled)
            {
                File.Delete(optionsFile);
                File.Delete(outputFile);
            }
            return results.ToArray();
        }

        private TestRunResults[] getResults(string outputFile, TestRunInfo[] runInfos)
        {
            var results = new List<TestRunResults>();
            var reader = new ResultXmlReader(outputFile);
            var tests = reader.Read();
            foreach (var byRunner in tests.GroupBy(x => x.Runner))
            {
                var runner = getRunnerFromType(byRunner.Key);
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

        private TestRunner getRunnerFromType(string type)
        {
            switch (type.ToLower())
            {
                case "nunit":
                    return TestRunner.NUnit;
                case "xunit":
                    return TestRunner.XUnit;
            }
            return TestRunner.Any;
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

        private void RunTests(string optionsFile, string outputFile)
        {
            try
            {
                var arguments = string.Format("--input=\"{0}\" --output=\"{1}\" --silent", optionsFile, outputFile);
                var exe = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "AutoTest.TestRunner.exe");
                DebugLog.Debug.WriteInfo("Running tests: {0} {1}", exe, arguments);
                var proc = new Process();
                proc.StartInfo = new ProcessStartInfo(exe, arguments);
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                var output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                if (output.Length > 0)
                    DebugLog.Debug.WriteError("AutoTest.TestRunner.exe failed with the following error" + Environment.NewLine + output);
            }
            catch (Exception ex)
            {
                DebugLog.Debug.WriteException(ex);
            }
        }

        private bool generateOptions(TestRunInfo[] runInfos, string file)
        {
            var plugins = new List<Plugin>();
            var options = new RunOptions();
            addNUnitTests(runInfos, plugins, options);
            addXUnitTests(runInfos, plugins, options);
            if (options.TestRuns.Count() == 0)
                return false;

            var writer = new OptionsXmlWriter(plugins, options);
            writer.Write(file);
            return true;
        }

        private void addNUnitTests(TestRunInfo[] runInfos, List<Plugin> plugins, RunOptions options)
        {
            var nunitInfos = runInfos.Where(x => canHandle(x, "nunit.framework", (ProjectDocument doc) => { return doc.ContainsNUnitTests; }));
            if (nunitInfos.Count() > 0)
            {
                plugins.Add(new Plugin(Path.GetFullPath(Path.Combine("TestRunners", "AutoTest.TestRunners.NUnit.dll")), "AutoTest.TestRunners.NUnit.Runner"));
                var runner = getRunnerOptions(nunitInfos, "NUnit", TestRunner.NUnit, getFramework, (TestRunInfo info) => { return _configuration.NunitTestRunner(getFramework(info)).Length > 0; });
                if (runner.Assemblies.Count() > 0)
                    options.AddTestRun(runner);
            }
        }

        private void addXUnitTests(TestRunInfo[] runInfos, List<Plugin> plugins, RunOptions options)
        {
            var xunitInfos = runInfos.Where(x => canHandle(x, "xunit", (ProjectDocument doc) => { return doc.ContainsXUnitTests; }));
            if (xunitInfos.Count() > 0)
            {
                plugins.Add(new Plugin(Path.GetFullPath(Path.Combine("TestRunners", "AutoTest.TestRunners.XUnit.dll")), "AutoTest.TestRunners.XUnit.Runner"));
                var runner = getRunnerOptions(xunitInfos, "XUnit", TestRunner.XUnit, getFramework, (TestRunInfo info) => { return _configuration.XunitTestRunner(getFramework(info)).Length > 0; });
                if (runner.Assemblies.Count() > 0)
                    options.AddTestRun(runner);
            }
        }

        private RunnerOptions getRunnerOptions(IEnumerable<TestRunInfo> unitInfos, string id, TestRunner testRunner, Func<TestRunInfo, string> frameworkEvaluator, Func<TestRunInfo, bool> hasOtherRunners)
        {
            var runner = new RunnerOptions(id);
            foreach (var info in unitInfos)
            {
                if (hasOtherRunners.Invoke(info))
                    continue;
                var assembly = new AssemblyOptions(info.Assembly, frameworkEvaluator.Invoke(info).Replace("v", ""));
                assembly.AddTests(info.GetTestsFor(testRunner));
                assembly.AddTests(info.GetTestsFor(TestRunner.Any));
                assembly.AddMembers(info.GetMembersFor(testRunner));
                assembly.AddMembers(info.GetMembersFor(TestRunner.Any));
                assembly.AddNamespaces(info.GetNamespacesFor(testRunner));
                assembly.AddNamespaces(info.GetNamespacesFor(TestRunner.Any));
                if (info.OnlyRunSpcifiedTestsFor(testRunner) && assembly.Tests.Count() == 0 && assembly.Members.Count() == 0 && assembly.Namespaces.Count() == 0)
                    continue;
                runner.AddAssembly(assembly);
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
            return info.Project.Value.Framework;
        }

        private bool canHandle(TestRunInfo info, string reference, Func<ProjectDocument, bool> frameworkValidator)
        {
            if (info.Project != null && info.Project.Value != null)
                return frameworkValidator.Invoke(info.Project.Value);
            return _referenceResolver.GetReferences(info.Assembly).Contains(reference);
        }
    }
}
