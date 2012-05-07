using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared.Errors;
using System.IO;
using System.Threading;
using System.Reflection;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners
{
    class TestRunner : MarshalByRefObject, ITestRunner
    {
        private List<TestResult> _results = new List<TestResult>();
        private List<string> _directories = new List<string>();
        private Dictionary<string, string> _assemblyCache = new Dictionary<string, string>();

        public void SetupResolver(bool silent, bool startLogger)
        {
            Logger.SetLogger(new ConsoleLogger(!silent, startLogger));
            _directories.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        public void Run(Plugin plugin, string id, RunSettings settings)
        {
            _directories.Add(Path.GetDirectoryName(settings.Assembly.Assembly));
            _directories.Add(Path.GetDirectoryName(plugin.Assembly));
            Logger.Debug("About to create plugin {0} in {1} for {2}", plugin.Type, plugin.Assembly, id);
            var runner = getRunner(plugin);
            var currentDirectory = Environment.CurrentDirectory;
            try
            {
                if (runner == null)
                    return;
				var isRunning = true;
				var client = new SocketClient();
				client.Connect(settings.ConnectOptions, (message) => {
					if (message == "load-assembly") {
						Logger.Debug(
							"Matching plugin identifier ({0}) to test identifier ({1})",
							runner.Identifier, id);
						if (!runner.Identifier.ToLower().Equals(id.ToLower()) && !id.ToLower().Equals("any"))
							return;
						Logger.Debug("Initializing channel");
						runner.SetLiveFeedbackChannel(new TestFeedbackProvider(client));
						var newCurrent = Path.GetDirectoryName(settings.Assembly.Assembly);
						Logger.Debug("Setting current directory to " + newCurrent);
						Environment.CurrentDirectory = newCurrent;
						runner.Prepare(settings.Assembly.Assembly, new string[] {});
					} else if (message == "run-all") {
						runTests(runner, new TestRunOptions());
					} else if (message == "exit") {
						isRunning = false;
					} else {
						var options = OptionsXmlReader.ParseOptions(message);
						runTests(runner, options);
					}
				});
				client.Send("RunnerID:" + settings.Assembly.Assembly + "|" + id.ToLower());
				while (isRunning)
					Thread.Sleep(10);
				client.Disconnect();
            }
            catch
            {
                throw;
            }
            finally
            {
                Environment.CurrentDirectory = currentDirectory;
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

		private void runTests(IAutoTestNetTestRunner runner, TestRunOptions options)
		{
			var verified = options.IsVerified ? "verified" : "unverified";
			Logger.Debug(
				"Running {4} tests for {0} " +
				"(tests:{1},members:{2}, namespaces:{3})",
				runner.Identifier,
				options.Tests.Count(),
				options.Members.Count(),
				options.Namespaces.Count(),
				verified);
			/*if (!options.IsVerified &&
				!runner.ContainsTestsFor(settings.Assembly.Assembly))
			{
				return;
			}*/
			var start = DateTime.Now;
			runner.RunTest(options);
			Logger.Debug("Tests finished in {0}ms", DateTime.Now.Subtract(start).TotalMilliseconds);
		}

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_assemblyCache.ContainsKey(args.Name))
            {
                if (_assemblyCache[args.Name] == "NotFound")
                    return null;
                else
                    return Assembly.LoadFrom(_assemblyCache[args.Name]);
            }
            foreach (var directory in _directories)
            {
                var file = Directory
					.GetFiles(directory)
					.Where(f => isMissingAssembly(args, f))
					.Select(x => x).FirstOrDefault();
                if (file == null)
                    continue;
                return Assembly.LoadFrom(file);
            }
            _assemblyCache.Add(args.Name, "NotFound");
            return null;
        }

        private bool isMissingAssembly(ResolveEventArgs args, string f)
        {
            try
            {
                if (_assemblyCache.ContainsValue(f))
                    return false;
                var name = Assembly.ReflectionOnlyLoadFrom(f).FullName;
                if (!_assemblyCache.ContainsKey(name))
                    _assemblyCache.Add(name, f);
                return name.Equals(args.Name);
            }
            catch
            {
                var key = "invalid signature for " + Path.GetFileName(f);
                if (!_assemblyCache.ContainsKey(key))
                    _assemblyCache.Add(key, f);
                return false;
            }
        }

        private IAutoTestNetTestRunner getRunner(Plugin plugin)
        {
            try
            {
                return plugin.New();
            }
            catch (Exception ex)
            {
                _results.Add(ErrorHandler.GetError(plugin.Type, ex));
            }
            return null;
        }
    }
}
