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
using System.Reflection;
using AutoTest.TestRunners.Shared.Logging;
using AutoTest.TestRunners.Shared.Communication;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners
{
    class TestRunner : MarshalByRefObject, ITestRunner
    {
        private List<TestResult> _results = new List<TestResult>();

        public IEnumerable<TestResult> Run(bool startLogger, Plugin plugin, string id, RunSettings settings)
        {
            if (startLogger)
                Logger.SetLogger(new ConsoleLogger());
            IEnumerable<TestResult> resultSet = null;
            var directories = new List<string>();
            directories.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            directories.Add(Path.GetDirectoryName(settings.Assembly.Assembly));
            directories.Add(Path.GetDirectoryName(plugin.Assembly));
            using (var resolver = new AssemblyResolver(directories.ToArray()))
            {
                Logger.Write("About to create plugin {0} in {1} for {2}", plugin.Type, plugin.Assembly, id);
                var runner = getRunner(plugin);
                var currentDirectory = Environment.CurrentDirectory;
                try
                {
                    if (runner == null)
                        return _results;
                    using (var server = new PipeServer(settings.PipeName))
                    {
                        Logger.Write("Matching plugin identifier ({0}) to test identifier ({1})", runner.Identifier, id);
                        if (!runner.Identifier.ToLower().Equals(id.ToLower()) && !id.ToLower().Equals("any"))
                            return _results;
                        Logger.Write("Checking whether assembly contains tests for {0}", id);
                        if (!settings.Assembly.IsVerified && !runner.ContainsTestsFor(settings.Assembly.Assembly))
                            return _results;
                        Logger.Write("Initializing channel");
                        runner.SetLiveFeedbackChannel(new TestFeedbackProvider(server));
                        var newCurrent = Path.GetDirectoryName(settings.Assembly.Assembly);
                        Logger.Write("Setting current directory to " + newCurrent);
                        Environment.CurrentDirectory = newCurrent;
                        Logger.Write("Starting test run");
                        resultSet = runner.Run(settings);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    Environment.CurrentDirectory = currentDirectory;
                }
            }
            return resultSet;
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
