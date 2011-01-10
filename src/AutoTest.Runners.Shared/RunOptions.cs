using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared
{
    public class RunOptions
    {
        private List<RunnerOptions> _runners = new List<RunnerOptions>();

        public IEnumerable<RunnerOptions> Runners { get { return _runners; } }

        public void AddRunner(RunnerOptions runner)
        {
            _runners.Add(runner);
        }

        public void AddRunners(IEnumerable<RunnerOptions> runners)
        {
            _runners.AddRange(runners);
        }
    }

    public class RunnerOptions
    {
        private List<AssemblyOptions> _assemblies = new List<AssemblyOptions>();
        private List<string> _categories = new List<string>();

        public IEnumerable<AssemblyOptions> Assemblies { get { return _assemblies; } }
        public IEnumerable<string> Categories { get { return _categories; } }

        public void AddAssembly(AssemblyOptions options)
        {
            _assemblies.Add(options);
        }

        public void AddAssemblies(IEnumerable<AssemblyOptions> options)
        {
            _assemblies.AddRange(options);
        }

        public void AddCategory(string category)
        {
            _categories.Add(category);
        }

        public void AddCategories(IEnumerable<string> categories)
        {
            _categories.AddRange(categories);
        }
    }

    public class AssemblyOptions
    {
        private List<string> _tests = new List<string>();

        public string Assembly { get; private set; }
        public IEnumerable<string> Tests { get { return _tests; } }
        public string Framework { get; private set; }

        public AssemblyOptions(string assembly)
        {
            Assembly = assembly;
            Framework = null;
        }

        public AssemblyOptions(string assembly, string framework)
        {
            Assembly = assembly;
            Framework = framework;
        }

        public AssemblyOptions(string assembly, IEnumerable<string> tests)
        {
            Assembly = assembly;
            Framework = null;
            _tests.AddRange(tests);
        }

        public AssemblyOptions(string assembly, string framework, IEnumerable<string> tests)
        {
            Assembly = assembly;
            Framework = framework;
            _tests.AddRange(tests);
        }
    }
}
