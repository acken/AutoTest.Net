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
	public class TestInstance
	{
		public string Assembly { get; private set; }
		public string Runner { get; private set; }

		public TestInstance(string assembly, string runner)
		{
			Assembly = assembly;
			Runner = runner;
		}
	}

	public class TestSession
	{
		private List<TestProcess> _processes = new List<TestProcess>();
		private Action<string> _logger = (s) => {};

		public TestInstance[] Instances {  
			get { 
				var runners = new List<TestInstance>();
				foreach (var proc in _processes)
					runners.AddRange(proc.GetRunners());
				return runners.ToArray();
			} 
		}

		internal TestSession(IEnumerable<TestProcess> processes, Action<string> logger)
		{
			_processes.AddRange(processes);
			_logger = logger;
		}

		public TestClient CreateClient(TestInstance instance, Func<bool> abortQuery)
		{
			return CreateClient(instance.Assembly, instance.Runner, abortQuery);
		}

		public TestClient CreateClient(string assembly, string runner, Func<bool> abortQuery)
		{
			var proc = _processes
				.FirstOrDefault(x => x.Hosts(assembly, runner));
			if (proc == null)
				return null;
			return proc.CreateClient(assembly, runner, abortQuery, _logger);
		}

		public void Kill()
		{
			_processes.ForEach(x => x.Kill());
		}
	}

	internal class TestProcess
	{
		private string _host;
		private int _port;
		private Process _proc;
		private TargetedRun _options;

		public TestProcess(Process proc, TargetedRun options, string host, int port)
		{
			_proc = proc;
			_options = options;
			_host = host;
			_port = port;
		}

		public bool Hosts(string assembly, string runner)
		{
			return _options.Runners
				.Any(x =>
					x.ID.ToLower() == runner.ToLower() &&
					x.Assemblies.Any(y => y.Assembly == assembly));
		}

		public IEnumerable<TestInstance> GetRunners()
		{
			var runners = new List<TestInstance>();
			foreach (var rnr in _options.Runners)
				runners.AddRange(rnr.Assemblies.Select(x => new TestInstance(x.Assembly, rnr.ID)));
			return runners;
		}

		public TestClient CreateClient(string assembly, string runner, Func<bool> abortQuery, Action<string> logger)
		{
			return new TestClient(
				_host,
				_port,
				assembly,
				runner,
				() => 
					_proc == null ||
					_proc.HasExited ||
					abortQuery(),
				logger);
		}

		public void Kill()
		{
            if (_proc == null)
                return;
            if (!_proc.HasExited)
            	_proc.Kill();
		}
	}

	public class TestClient
	{
		private SocketClient _client = new SocketClient();
		private string _assembly;
		private string _runner;
		private string _runnerID;
		private Func<bool> _abortQuery;
		private Action<string> _onTestStart;
		private Action<Results.TestResult> _onTestFinished;
		private int _testsRan = -1;
		private List<Results.TestResult> _results = new List<Results.TestResult>();
		private Action<string> _logger = (s) => {};

		public bool IsConnected {
			get {
				if (_client == null)
					return false;
				return _client.IsConnected;
			}
		}

		public TestClient(string host, int port, string assembly, string runner, Func<bool> abortQuery, Action<string> logger)
		{
			_client.LogTo(logger);
			_assembly = assembly;
			_runner = runner;
			_runnerID = string.Format("{0}|{1}:", _assembly, _runner.ToLower());
			_abortQuery = abortQuery;
			_logger = logger;
			_client.Connect(new ConnectionOptions(host, port), handleMessage);
		}

		public void Load()
		{
			_client.SendAndWait(string.Format("{0}load-assembly", _runnerID));
		}

		public List<Results.TestResult> RunAllTests(
			Action<string> testStart,
			Action<Results.TestResult> testFinished)
		{
			return runTests("run-all", testStart, testFinished);
		}

		public List<Results.TestResult> RunTests(
			TestRunOptions options,
			Action<string> testStart,
			Action<Results.TestResult> testFinished)
		{
			var message = OptionsXmlWriter.WriteOptions(options);
			return runTests(message, testStart, testFinished);
		}

		private List<Results.TestResult> runTests(
			string message,
			Action<string> testStart,
			Action<Results.TestResult> testFinished)
		{
			_onTestStart = testStart;
			_onTestFinished = testFinished;
			_testsRan = -1;
			_results.Clear();
			_logger(string.Format("Sending: {0}{1}", _runnerID, message));
			_client.Send(string.Format("{0}{1}", _runnerID, message));
			while (!_abortQuery() && ( _testsRan == -1 || _testsRan != _results.Count))
				Thread.Sleep(5);
			return _results;
		}

		public void Exit()
		{
			if (!_abortQuery())
				_client.SendAndWait("exit");
		}

		private void handleMessage(string message)
		{
			if (message == null)
				return;
			try
			{
				if (message.StartsWith(_runnerID + "Run finished:")) {
					_testsRan = int.Parse(getMessage(message, _runnerID + "Run finished:"));
				} else if (message.StartsWith(_runnerID + "Run started:")) {
					_testsRan = _testsRan;
				} else if (message.StartsWith(_runnerID + "Test started:")) {
					if (_onTestStart != null)
						_onTestStart(getMessage(message, _runnerID + "Test started:"));
				} else if (message.StartsWith(_runnerID + "<?xml")) {
					var result = Results.TestResult.FromXml(getMessage(message, _runnerID));
					if (result == null) {
						return;
					}
					_results.Add(result);
					if (_onTestFinished != null)
						_onTestFinished(result);
				}
			}
			catch (Exception ex)
			{
				_logger("Crashed while handling message: " + message);
				_logger(ex.ToString());
			}
		}

		private string getMessage(string message, string prefix)
		{
			return message.Substring(prefix.Length, message.Length - prefix.Length);
		}
	}

    class TestProcessLauncher
    {
        private ITestRunProcessFeedback _feedback;
        private TargetedRun _targetedRun;
        private bool _runInParallel = false;
        private bool _startSuspended = false;
        private Func<bool> _abortWhen = null;
		private string _logFile = null;
        private Action<string> _logger = (s) => {};
        private Action<Platform, Version, Action<ProcessStartInfo, bool>> _processWrapper = null;
        private bool _compatabilityMode = false;
        private string _executable;
        private string _input;
        private string _output;
        private PipeClient _pipeClient = null;

        public TestProcessLauncher(TargetedRun targetedRun, ITestRunProcessFeedback feedback)
        {
            _targetedRun = targetedRun;
            _feedback = feedback;
        }

        public TestProcessLauncher RunParallel()
        {
            _runInParallel = true;
            return this;
        }

        public TestProcessLauncher StartSuspended()
        {
            _startSuspended = true;
            return this;
        }

        public TestProcessLauncher AbortWhen(Func<bool> abortWhen)
        {
            _abortWhen = abortWhen;
            return this;
        }

        public TestProcessLauncher WrapTestProcessWith(Action<Platform, Version, Action<ProcessStartInfo, bool>> processWrapper)
        {
            _processWrapper = processWrapper;
            return this;
        }

        public TestProcessLauncher RunInCompatibilityMode()
        {
            _compatabilityMode = true;
            return this;
        }

        public TestProcessLauncher SetInternalLoggerTo(Action<string> logger)
		{
			_logger = logger;
			return this;
		}

		public TestProcessLauncher LogTo(string fileName)
		{
			_logFile = fileName;
			return this;
		}
		 
		private TestProcess _currentProcess = null;
		private object _processLock = new object();
        public TestProcess Start()
        {
			lock (_processLock) {
				_currentProcess = null;
				_executable = getExecutable();
				_input = createInputFile();
				_output = Path.GetTempFileName();
				runProcess();
				return _currentProcess;
			}
        }

        private void runProcess()
        {
            if (_processWrapper == null)
                run(new ProcessStartInfo(), false);
            else
                _processWrapper.Invoke(_targetedRun.Platform, _targetedRun.TargetFramework, run);
        }

        private void run(ProcessStartInfo startInfo, bool doNotshellExecute)
        {
            //var listener = startChannelListener(channel);
            var arguments = string.Format("--input=\"{0}\"", _input);
            if (_runInParallel)
                arguments += " --run_assemblies_parallel";
            if (_startSuspended)
                arguments += " --startsuspended";
            if (_compatabilityMode)
                arguments += " --compatibility-mode";
			if (_logFile != null)
				arguments += " --logging=" + _logFile;
			else
				arguments += " --silent";

			var connectionFile = Path.GetTempFileName();
			arguments += " --connectioninfo=\"" + connectionFile + "\"";
            var proc = new Process();
            proc.StartInfo = startInfo;
			if (OS.IsPosix)
			{
				proc.StartInfo.FileName = "mono";
				proc.StartInfo.Arguments =  " --debug " + _executable + " " + arguments;
			}
			else
			{
            	proc.StartInfo.FileName = _executable;
				proc.StartInfo.Arguments = arguments;
			}

            if (_feedback != null)
                _feedback.ProcessStart(proc.StartInfo.FileName + " " + proc.StartInfo.Arguments);

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = !doNotshellExecute;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(_executable);
            _logger("Running: " + _executable + " " + arguments + " in " + proc.StartInfo.WorkingDirectory);
            proc.Start();

			while (!File.Exists(connectionFile) || File.ReadAllText(connectionFile).Length == 0)
				Thread.Sleep(10);
			
			var content = File.ReadAllText(connectionFile);
			Console.WriteLine(content);
			var chunks = content.Split(new[] { ":" }, StringSplitOptions.None);
			_currentProcess = new TestProcess(proc, _targetedRun, chunks[0], int.Parse(chunks[1]));

            //var abortListener = new System.Threading.Thread(listenForAborts);
            //abortListener.Start();
            //_proc.WaitForExit();
            //closeClient();
            //if (listener != null)
            //    listener.Join();
            //abortListener.Join();
            //if (aborted())
            //    return;
            //var results = getResults(_output);
            //TestRunProcess.AddResults(results);
        }

        private Thread startChannelListener(string channel)
        {
			if (OS.IsPosix)
				return null;
            var thread = new Thread(
                (x) => 
                    {
                        _pipeClient = new PipeClient();
                        _pipeClient.Listen(
                                x.ToString(),
                                (msg) => 
                                    {
                                        if (msg == "")
                                            return;
                                        if (_feedback != null)
                                        {
                                            var testStarted = "Test started:";
                                            if (msg.StartsWith(testStarted))
                                                _feedback.TestStarted(msg.Substring(testStarted.Length, msg.Length - testStarted.Length));
                                            else
                                                _feedback.TestFinished(TestResult.FromXml(msg));
                                        }
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
			// TODO Fix this
            /*if (_proc == null)
                return;
            while (!_proc.HasExited)
            {
                if (_abortWhen.Invoke())
                {
                    closeClient();
                    _proc.Kill();
                    return;
                }
                System.Threading.Thread.Sleep(10);
            }*/
        }

        private void closeClient()
        {
            if (_pipeClient != null)
            {
                _pipeClient.Disconnect();
                _pipeClient = null;
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
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRunners");
            return new PluginLocator(path).GetPluginsFrom(options);
        }

        private string getExecutable()
        {
            return new TestProcessSelector().Get(_targetedRun.Platform, _targetedRun.TargetFramework);
        }
    }
}
