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
using Mono.Cecil;
using AutoTest.TestRunners.Shared.Logging;

namespace AutoTest.TestRunners
{
    class TestRunner : MarshalByRefObject, ITestRunner
    {
        private List<TestResult> _results = new List<TestResult>();
        private List<string> _directories = new List<string>();
        private Dictionary<string, string> _assemblyCache = new Dictionary<string, string>();

        public void SetupResolver(bool startLogger)
        {
            if (startLogger)
                Logger.SetLogger(new ConsoleLogger());
            _directories.Add(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath));
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        public IEnumerable<TestResult> Run(Plugin plugin, string id, RunSettings settings)
        {
            _directories.Add(Path.GetDirectoryName(settings.Assembly.Assembly));
            _directories.Add(Path.GetDirectoryName(plugin.Assembly));
            Logger.Write("About to create plugin {0} in {1} for {2}", plugin.Type, plugin.Assembly, id);
            var runner = getRunner(plugin);
            try
            {
                if (runner == null)
                    return _results;
                Logger.Write("Matching plugin identifier ({0}) to test identifier ({1})", runner.Identifier, id);
                if (!runner.Identifier.ToLower().Equals(id.ToLower()) && !id.ToLower().Equals("any"))
                    return _results;
                Logger.Write("Checking whether assembly contains tests for {0}", id);
                if (!runner.ContainsTestsFor(settings.Assembly.Assembly))
                    return _results;
                Logger.Write("Starting test run");
                return runner.Run(settings);
            }
            catch
            {
                throw;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
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
                var file = Directory.GetFiles(directory).Where(f => isMissingAssembly(args, f)).Select(x => x).FirstOrDefault();
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
                var assembly = AssemblyDefinition.ReadAssembly(f);
                if (!_assemblyCache.ContainsKey(assembly.FullName))
                    _assemblyCache.Add(assembly.FullName, f);
                return assembly.FullName.Equals(args.Name);
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
