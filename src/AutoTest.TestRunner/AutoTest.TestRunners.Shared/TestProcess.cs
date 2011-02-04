using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Targeting;
using System.IO;
using System.Reflection;
using AutoTest.TestRunners.Shared.Plugins;
using System.Diagnostics;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Errors;

namespace AutoTest.TestRunners.Shared
{
    class TestProcess
    {
        private ITestRunProcessFeedback _feedback;
        private TargetedRun _targetedRun;
        private bool _runInParallel = false;

        public TestProcess(TargetedRun targetedRun, ITestRunProcessFeedback feedback)
        {
            _targetedRun = targetedRun;
            _feedback = feedback;
        }

        public TestProcess RunParallel()
        {
            _runInParallel = true;
            return this;
        }

        public void Start()
        {
            var executable = getExecutable();
            var input = createInputFile();
            var output = Path.GetTempFileName();
            runProcess(executable, input, output);
        }

        private void runProcess(string executable, string input, string output)
        {
            var arguments = string.Format("--input=\"{0}\" --output=\"{1}\" --silent", input, output);
            if (_runInParallel)
                arguments += " --run_assemblies_parallel";
            if (_feedback != null)
                _feedback.ProcessStart(executable + " " + arguments);
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(executable, arguments);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(executable);
            proc.Start();
            var consoleOutput = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            var results = getResults(output);
            if (consoleOutput.Length > 0)
                results.Add(ErrorHandler.GetError(consoleOutput));
            TestRunProcess.AddResults(results);
        }

        private List<TestResult> getResults(string output)
        {
            var results = new List<TestResult>();
            if (File.Exists(output))
                results.AddRange(getResultsFromFile(output));
            else
                results.Add(ErrorHandler.GetError("Could not find output file " + output));
            return results;
        }

        private IEnumerable<TestResult> getResultsFromFile(string output)
        {
            var reader = new ResultXmlReader(output);
            return reader.Read();
        }

        private RunOptions getRunOptions()
        {
            var options = new RunOptions();
            foreach (var run in _targetedRun.Runners)
                options.AddTestRun(run);
            return options;
        }

        private string createInputFile()
        {
            var options = getRunOptions();
            var file = Path.GetTempFileName();
            var writer = new OptionsXmlWriter(getPlugins(options), options);
            writer.Write(file);
            return file;
        }

        private IEnumerable<Plugin> getPlugins(RunOptions options)
        {
            var path = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "TestRunners");
            return new PluginLocator(path).GetPluginsFrom(options);
        }

        private string getExecutable()
        {
            return new TestProcessSelector().Get(_targetedRun.Platform, _targetedRun.TargetFramework);
        }
    }
}
