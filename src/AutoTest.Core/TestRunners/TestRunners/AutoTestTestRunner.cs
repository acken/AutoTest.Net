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

            var optionsFile = Path.GetTempFileName();
            var outputFile = Path.GetTempFileName();
            if (!generateOptions(runInfos, optionsFile))
                return new TestRunResults[] { };

            DebugLog.Debug.WriteMessage(File.ReadAllText(optionsFile));
            RunTests(optionsFile, outputFile);
            DebugLog.Debug.WriteMessage(File.ReadAllText(outputFile));
            var results = getResults(outputFile, runInfos);
            
            File.Delete(optionsFile);
            File.Delete(outputFile);
            return results.ToArray();
        }

        private TestRunResults[] getResults(string outputFile, TestRunInfo[] runInfos)
        {
            var results = new List<TestRunResults>();
            var reader = new ResultXmlReader(outputFile);
            var tests = reader.Read();
            foreach (var byRunner in tests.GroupBy(x => x.Runner))
            {
                var runner = TestRunner.NUnit;
                foreach (var byAssembly in byRunner.GroupBy(x => x.Assembly))
                {
                    var info = runInfos.Where(x => x.Assembly.Equals(byAssembly.Key)).FirstOrDefault();
                    var project = "";
                    var partial = false;
                    if (info != null)
                    {
                        if (info.Project != null)
                            project = info.Project.Key;
                        partial = info.OnlyRunSpcifiedTestsFor(runner) || info.GetTestsFor(runner).Count() > 0;
                    }
                    results.Add(new TestRunResults(
                        project,
                        byAssembly.Key,
                        partial,
                        runner,
                        byAssembly.Select(x => new Messages.TestResult(
                            runner,
                            getTestState(x.State),
                            x.TestName,
                            x.Message,
                            x.StackLines.Select(y => (IStackLine) new StackLineMessage(y.Method, y.File, y.Line)).ToArray<IStackLine>()
                            )).ToArray()
                            ));
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

        private void RunTests(string optionsFile, string outputFile)
        {
            var arguments = string.Format("\"{0}\" \"{1}\"", optionsFile, outputFile);
            var exe = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "AutoTest.TestRunner.exe");
            DebugLog.Debug.WriteMessage(string.Format("Running tests: {0} {1}", exe, arguments));
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
                DebugLog.Debug.WriteMessage("AutoTest.TestRunner.exe failed with the following error" + Environment.NewLine + output);
        }

        private bool generateOptions(TestRunInfo[] runInfos, string file)
        {
            var plugins = new List<Plugin>();
            var options = new RunOptions();
            addNUnitTests(runInfos, plugins, options);
            if (options.TestRuns.Count() == 0)
                return false;

            var writer = new OptionsXmlWriter(plugins, options);
            writer.Write(file);
            return true;
        }

        private void addNUnitTests(TestRunInfo[] runInfos, List<Plugin> plugins, RunOptions options)
        {
            var nunitInfos = runInfos.Where(x => canHandleNUnit(x));
            if (nunitInfos.Count() > 0)
            {
                plugins.Add(new Plugin(Path.GetFullPath(Path.Combine("TestRunners", "AutoTest.TestRunners.NUnit.dll")), "AutoTest.TestRunners.NUnit.Runner"));
                var runner = getNUnitRunnerOptions(nunitInfos, "NUnit", TestRunner.NUnit, getFramework);
                if (runner.Assemblies.Count() > 0)
                    options.AddTestRun(runner);
            }
        }

        private RunnerOptions getNUnitRunnerOptions(IEnumerable<TestRunInfo> nunitInfos, string id, TestRunner testRunner, Func<TestRunInfo, string> frameworkEvaluator)
        {
            var runner = new RunnerOptions(id);
            foreach (var info in nunitInfos)
            {
                if (hasOtherNUnitRunners(info))
                    continue;
                var assembly = new AssemblyOptions(info.Assembly, frameworkEvaluator.Invoke(info).Replace("v", ""));
                assembly.AddTests(info.GetTestsFor(testRunner));
                assembly.AddTests(info.GetTestsFor(TestRunner.Any));
                if (info.OnlyRunSpcifiedTestsFor(testRunner) && assembly.Tests.Count() == 0)
                    continue;
                runner.AddAssembly(assembly);
            }
            return runner;
        }

        private bool hasOtherNUnitRunners(TestRunInfo info)
        {
            return _configuration.NunitTestRunner(getFramework(info)).Length > 0;
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
