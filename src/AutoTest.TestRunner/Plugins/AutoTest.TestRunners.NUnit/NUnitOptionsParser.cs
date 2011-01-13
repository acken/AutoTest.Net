using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestRunners.NUnit
{
    class NUnitOptionsParser
    {
        private RunnerOptions _runnerOptions;
        private List<Options> _options = new List<Options>();

        public IEnumerable<Options> Options { get { return _options; } }

        public NUnitOptionsParser(RunnerOptions options)
        {
            _runnerOptions = options;
        }

        public void Parse()
        {
            var query = _runnerOptions.Assemblies
                .GroupBy(x => x.Framework)
                .Select(x => x);
            foreach (var item in query)
                addOptions(item);
        }

        private void addOptions(IGrouping<string, AssemblyOptions> x)
        {
            var options = new Options(
                    getAssemblies(x),
                    getCategories(),
                    x.First().Framework,
                    getTests(x)
                );
            _options.Add(options);
        }

        private string getAssemblies(IGrouping<string, AssemblyOptions> x)
        {
            var assemblies = "";
            foreach (var item in x)
                assemblies += assemblies.Length.Equals(0) ? item.Assembly : string.Format(",{0}", item.Assembly);
            return assemblies;
        }

        private string getCategories()
        {
            var categories = "";
            foreach (var category in _runnerOptions.Categories)
                categories += categories.Length.Equals(0) ? category : string.Format(",{0}", category);
            return categories;
        }

        private string getTests(IGrouping<string, AssemblyOptions> x)
        {
            var tests = "";
            foreach (var item in x)
            {
                foreach (var test in item.Tests)
                    tests += tests.Length.Equals(0) ? test : string.Format(",{0}", test);
                foreach (var member in item.Members)
                    tests += tests.Length.Equals(0) ? member : string.Format(",{0}", member);
                foreach (var ns in item.Namespaces)
                    tests += tests.Length.Equals(0) ? ns : string.Format(",{0}", ns);
            }
            return tests;
        }
    }

    class Options
    {
        public string Assemblies { get; private set; }
        public string Categories { get; private set; }
        public string Framework { get; private set; }
        public string Tests { get; private set; }

        public Options(string assemblies, string categories, string framework, string tests)
        {
            Assemblies = assemblies;
            Categories = categories;
            Framework = framework;
            Tests = tests;
        }
    }
}
