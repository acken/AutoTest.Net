using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
using System.Runtime.Remoting;
using AutoTest.TestRunners.Shared.Errors;
using System.Reflection;
using System.Threading;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;

namespace AutoTest.TestRunners
{
    class Program
    {
        private static Arguments _arguments;
        private static List<Thread> _haltedThreads = new List<Thread>();
		private static SocketClient _client = new SocketClient();

		public static ITestFeedbackProvider Channel;
        
        [STAThread]
        static void Main(string[] args)
        {
			if (!args.Any(x => x.StartsWith("--input=")) &&
				args.Length == 3)
			{
				int port;
				if (int.TryParse(args[1], out port)) {
					var client = new SocketClient();
					client.Connect(new ConnectionOptions(args[0], port), (meh) => {});
					client.Send(args[2]);
					Console.Write("");
				}
				return;
			}

			Channel = new TestFeedbackProvider(_client);
            var parser = new ArgumentParser(args);
			_arguments = parser.Parse();
            Logger.SetLogger(new ConsoleLogger(!_arguments.Silent, !_arguments.Silent && _arguments.Logging));
            writeHeader();
            if (!File.Exists(_arguments.InputFile))
            {
                printUseage();
                return;
            }
            Logger.Debug("Test run options:");
            Logger.Debug(File.ReadAllText(_arguments.InputFile));
            if (_arguments.StartSuspended)
                Console.ReadLine();
            tryRunTests();

			// We do this since NUnit threads some times keep staing in running mode even after finished.
            killHaltedThreads();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private static void killHaltedThreads()
        {
            lock (_haltedThreads)
            {
                if (_haltedThreads.Count == 0)
                    return;
                foreach (var thread in _haltedThreads)
                    thread.Abort();
                Thread.Sleep(100);
            }
        }

        private static void writeHeader()
        {
            Write("AutoTest.TestRunner v1.0");
            Write("Author - Svein Arne Ackenhausen");
            Write("AutoTest.TestRunner is a plugin based generic test runner. ");
            Write("");
        }

        private static void tryRunTests()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledExceptionHandler;
			var options = parseOptions();
			if (options != null) {
				try {
                    prepareRunners(options);
                } catch (Exception ex) {
                    try {
                        Channel.TestFinished(ErrorHandler.GetError("Init", ex));
                    } catch {
                        Console.WriteLine(ex.ToString());
                    }
                }
			}
			AppDomain.CurrentDomain.UnhandledException -= CurrentDomainUnhandledExceptionHandler;
		}

		private static void prepareRunners(OptionsXmlReader options)
		{
			var runners = new List<Thread>();
			var server = new TcpServer();
			server.Start(_arguments.Port);
			var serverInfo = string.Format("{0}:{1}", server.Server, server.Port);
			Console.WriteLine("Listening on: {0}", serverInfo);
			if (_arguments.ConnectionInfo != null)
				File.WriteAllText(_arguments.ConnectionInfo, serverInfo);
			_client.Connect(new ConnectionOptions(server.Server, server.Port), (msg) => {});
			foreach (var plugin in getPlugins(options))
			{
				var instance = plugin.New();
				foreach (var currentRuns in options 
												.Options
												.TestRuns
												.Where(x =>
													x.ID.ToLower().Equals("any") ||
													instance.Handles(x.ID)))
				{
					var run = currentRuns;
					if (run == null)
						continue;
					foreach (var assembly in run.Assemblies)
					{
						var process = new SubDomainRunner(
							plugin,
							run.ID,
							run.Categories,
							assembly,
							_arguments.Silent,
							!_arguments.Silent && _arguments.Logging,
							new ConnectionOptions(server.Server, server.Port),
							_arguments.CompatabilityMode);
						var runner = new Thread(() => process.Run(null));
						runners.Add(runner);
						runner.Start();
					}
				}
			}

			runners.ForEach(x => x.Join());
			_client.Disconnect();
			if (_arguments.ConnectionInfo != null && File.Exists(_arguments.ConnectionInfo))
				File.Delete(_arguments.ConnectionInfo);
		}

		private static OptionsXmlReader parseOptions() {
			var parser = new OptionsXmlReader();
			parser.ParseFile(_arguments.InputFile);
			if (parser.IsValid)
				return parser;
			return null;
		}

        public static void CurrentDomainUnhandledExceptionHandler(
			object sender,
			UnhandledExceptionEventArgs args)
        {
            var message = getException((Exception)args.ExceptionObject);

            // TODO: Seriously!? Firgure out what thread is causing the app domain unload exception
			// Yeah, seriously. When user code throws background exceptions we want them to know.
            if (!_arguments.CompatabilityMode &&
				!args.ExceptionObject.GetType().Equals(typeof(System.AppDomainUnloadedException)))
            {
                var finalOutput = new TestResult("Any", "", "", 0, 
					"An unhandled exception was thrown while running a test.", TestState.Panic,
                    "This is usually caused by a background thread throwing an unhandled exception. " +
                    "Most test runners run in clr 1 mode which hides these exceptions from you. " +
					"If you still want to suppress these errors (not recommended) you can enable " +
					"compatibility mode." + Environment.NewLine + Environment.NewLine +
                    "To head on to happy land where fluffy bunnies roam freely painting green left " +
					"right and center do so by passing the --compatibility-mode switch to the test " +
                    "runner or set the <TestRunnerCompatibilityMode>true</TestRunnerCompatibilityMode> " +
					"configuration option in AutoTest.Net." +
					Environment.NewLine + Environment.NewLine + message);
                Channel.TestFinished(finalOutput);
            }

            if (args.IsTerminating)
                Environment.Exit(-1);

            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Name = "Dead thread";
            lock (_haltedThreads)
            {
                _haltedThreads.Add(Thread.CurrentThread);
            }
        }

        private static string getException(Exception ex)
        {
            var text = ex.ToString();
            if (ex.InnerException != null)
                text += getException(ex.InnerException);
            return text;
        }

        private static void printUseage()
        {
            Write("Start runner syntax: AutoTest.TestRunner.exe --input=options_file " +
				  "[--connectioninfo=file_to_write_connect_info_to] " +
				  "[--startsuspended] [--silent] [--logging] [--compatibility-mode] [--port=PORT]");
			Write("Send message syntax: IP PORT MESSAGE");
            Write("");
			Write("Options file format");
            Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            Write("<run>");
            Write("\t<!--It can contain 0-n plugins. If 0 the runner will load all available plugins-->");
            Write("\t<plugin type=\"Plugin.IAutoTestNetTestRunner.Implementation\">" +
				"C:\\Some\\Path\\PluginAssembly.dll</plugin>");
            Write("\t<!--It can contain 0-n runners. The id is what determines which runner " +
				"will handle that run-->");
            Write("\t<runner id=\"NUnit\">");
            Write("\t\t<!--It can contain 0-n categories to ignore-->");
            Write("\t\t<categories>");
            Write("\t\t\t<ignore_category>IgnoreCategory</ignore_category>");
            Write("\t\t</categories>");
            Write("\t\t<!--It can contain 1-n assemblies to test.-->");
            Write("\t\t<test_assembly name=\"C:\\my\\testassembly.dll\" />");
            Write("\t</runner>");
            Write("</run>");
			Write("");
			Write("");
            Write("<===================== Commands for a running test runner instance ======================>");
			Write("When communicating with a running instance of a test runner you will use a socket endpoint");
			Write("using null terminated text commands. You will recieve test feedback from the same tcp ");
			Write("channel. Assembly and test runner information from the options file is used to launch ");
			Write("idle runners. The runner id is [assembly full path]|[plugin name in lower case].");
			Write("");
			Write("<== Command string format: RUNNERID:MESSAGE ==>");
			Write("Initializing the runner (load-assembly): /my/path/myasm.dll|nunit:load-assembly");
			Write("Run all tests (run-all): /my/path/myasm.dll|nunit:run-all");
			Write("Run tests (test run xml): /my/path/myasm.dll|nunit:XML");
			Write("Exit runner (exit): /my/path/myasm.dll|nunit:exit");
			Write("Exit all runner (exit): exit");
			Write("");
			Write("Test run format");
			Write("<!--If verified=true the runner will not verify that tests belong to framework-->");
			Write("<test_run verified=\"true\">");
			Write("\t<!--It can contain 0-n tests-->");
            Write("\t<tests>");
            Write("\t\t<test>testassembly.class.test1</test>");
            Write("\t</tests>");
            Write("\t<!--It can contain 0-n members-->");
            Write("\t<members>");
            Write("\t\t<member>testassembly.class2</member>");
            Write("\t</members>");
            Write("\t<!--It can contain 0-n namespaces-->");
            Write("\t<namespaces>");
            Write("\t\t<namespace>testassembly.somenamespace1</namespace>");
            Write("\t</namespaces>");
            Write("</test_run>");
        }
		
        private static IEnumerable<Plugin> getPlugins(OptionsXmlReader parser)
        {
            if (parser.Plugins.Count() == 0)
                return allPlugins();
            return parser.Plugins;
        }

        private static IEnumerable<Plugin> allPlugins()
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dir = Path.Combine(currentDir, "TestRunners");
            if (!Directory.Exists(dir))
                return new Plugin[] { };
            var locator = new PluginLocator(dir);
            return locator.Locate();
        }

        public static void WriteNow(string message)
        {
            Write(string.Format("{0} {1}", DateTime.Now.ToLongTimeString(), message));
        }

        public static void Write(string message)
        {
            if (!_arguments.Silent)
                Console.WriteLine(message);
        }
    }
}
