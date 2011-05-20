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
using System.Threading;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners.Shared
{
    class TestProcess
    {
        private ITestRunProcessFeedback _feedback;
        private TargetedRun _targetedRun;
        private bool _runInParallel = false;
        private bool _startSuspended = false;
		private bool _activateProfiling = false;
		private string _profilerLogFile = null;
		private string _profileIncludes = null;
        private Func<bool> _abortWhen = null;
        private Process _proc = null;

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

        public TestProcess StartSuspended()
        {
            _startSuspended = true;
            return this;
        }

        public TestProcess AbortWhen(Func<bool> abortWhen)
        {
            _abortWhen = abortWhen;
            return this;
        }
		
		public TestProcess ActivateProfilingFor(string logFileName)
		{
			_profilerLogFile = logFileName;
			_activateProfiling = true;
			return this;
		}
		
		public TestProcess IncludeForProfiling(string excludelist)
		{
			_profileIncludes = excludelist;
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
            var channel = Guid.NewGuid().ToString();
            var listener = startChannelListener(channel);
            var arguments = string.Format("--input=\"{0}\" --output=\"{1}\" --silent --channel=\"{2}\"", input, output, channel);
            if (_runInParallel)
                arguments += " --run_assemblies_parallel";
            if (_startSuspended)
                arguments += " --startsuspended";
            if (_feedback != null)
                _feedback.ProcessStart(executable + " " + arguments);
            _proc = new Process();
            _proc.StartInfo = new ProcessStartInfo(executable, arguments);
            _proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _proc.StartInfo.RedirectStandardOutput = true;
            _proc.StartInfo.UseShellExecute = false;
            _proc.StartInfo.CreateNoWindow = true;
            _proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(executable);
			if (_activateProfiling)
			{
				if (_targetedRun.Platform == Platform.AnyCPU)
                	_proc.StartInfo.EnvironmentVariables.Add("Cor_Profiler", "{5D789D88-EEE7-46C4-909F-E39D5606544D}");
				else
                	_proc.StartInfo.EnvironmentVariables.Add("Cor_Profiler", "{36C8D782-F697-45C4-856A-92D05C061A39}");
                _proc.StartInfo.EnvironmentVariables.Add("Cor_Enable_Profiling", "1");
                _proc.StartInfo.EnvironmentVariables.Add("MMProfiler_LogFilename", _profilerLogFile);
				
				if (_profileIncludes != null)
                    _proc.StartInfo.EnvironmentVariables.Add("MMProfiler_Includes", _profileIncludes);
                _proc.StartInfo.EnvironmentVariables.Add("MMProfiler_Runtime", string.Format(".NET{0}", _targetedRun.TargetFramework.Major));
			}
            _proc.Start();
            new System.Threading.Thread(listenForAborts).Start();
            _proc.StandardOutput.ReadToEnd();
            _proc.WaitForExit();
            if (aborted())
            {
                listener.Abort();
                return;
            }
            var results = getResults(output);
            TestRunProcess.AddResults(results);
        }

        private Thread startChannelListener(string channel)
        {
            var thread = new Thread(
                (x) => 
                    { 
                        new PipeClient()
                            .Listen(
                                x.ToString(),
                                (msg) => 
                                    { 
                                        _feedback.TestFinished(new TestResult("", "", "", 0, msg, TestState.Failed, ""));
                                    });
                    });
            thread.Start(channel);
            return thread;
        }

        private bool aborted()
        {
            if (_abortWhen == null)
                return false;
            return _abortWhen.Invoke();
        }

        private void listenForAborts()
        {
            if (_abortWhen == null)
                return;
            if (_proc == null)
                return;
            while (!_proc.HasExited)
            {
                if (_abortWhen.Invoke())
                {
                    _proc.Kill();
                    return;
                }
                System.Threading.Thread.Sleep(10);
            }
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
