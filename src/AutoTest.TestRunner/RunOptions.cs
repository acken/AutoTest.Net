using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners
{
    class RunOptions
    {
        private List<RunnerOptions> _runners = new List<RunnerOptions>();
        private List<string> _categories = new List<string>();

        public IEnumerable<RunnerOptions> Runners { get { return _runners; } }

        public void AddRunner(RunnerOptions runner)
        {
            _runners.Add(runner);
        }

        public void AddCategories(IEnumerable<string> categories)
        {
            _categories.AddRange(categories);
        }
    }

    class RunnerOptions
    {
        private List<AssemblyOptions> _assemblies = new List<AssemblyOptions>();

        public IEnumerable<AssemblyOptions> Assemblies { get { return _assemblies; } }
    }

    class AssemblyOptions
    {
        private List<string> _tests = new List<string>();

        public IEnumerable<string> Tests { get { return _tests; } }
        public string Framework { get; private set; }

        public AssemblyOptions(string framework)
        {
            Framework = framework;
        }
    }
}
