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

namespace AutoTest.TestRunners
{
    class TestRunner : MarshalByRefObject, ITestRunner
    {
        private List<TestResult> _results = new List<TestResult>();

        public IEnumerable<TestResult> Run(Plugin plugin, string id, RunSettings settings)
        {
            var runner = getRunner(plugin);
            if (runner == null)
                return _results;
            if (!runner.Identifier.ToLower().Equals(id.ToLower()))
                return _results;
            if (!runner.ContainsTestsFor(settings.Assembly.Assembly))
                return _results;
            return runner.Run(settings);
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
