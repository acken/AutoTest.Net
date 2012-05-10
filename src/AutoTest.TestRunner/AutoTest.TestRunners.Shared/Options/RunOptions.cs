using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.Options
{
    [Serializable]
    public class RunOptions
    {
        private List<RunnerOptions> _runners = new List<RunnerOptions>();

        public IEnumerable<RunnerOptions> TestRuns { get { return _runners; } }

        public RunOptions AddTestRun(RunnerOptions runner)
        {
            _runners.Add(runner);
			return this;
        }

        public RunOptions AddTestRun(IEnumerable<RunnerOptions> runners)
        {
            _runners.AddRange(runners);
			return this;
        }
    }

    [Serializable]
    public class RunnerOptions
    {
        private List<AssemblyOptions> _assemblies = new List<AssemblyOptions>();
        private List<string> _categories = new List<string>();

        public string ID { get; private set; }

        public IEnumerable<AssemblyOptions> Assemblies { get { return _assemblies; } }
        public IEnumerable<string> Categories { get { return _categories; } }

        public RunnerOptions(string id)
        {
            ID = id;
        }

        public RunnerOptions AddAssembly(AssemblyOptions options)
        {
            _assemblies.Add(options);
			return this;
        }

        public RunnerOptions AddAssemblies(AssemblyOptions[] options)
        {
            _assemblies.AddRange(options);
			return this;
        }

        public RunnerOptions AddCategory(string category)
        {
            _categories.Add(category);
			return this;
        }

        public RunnerOptions AddCategories(string[] categories)
        {
            _categories.AddRange(categories);
			return this;
        }
    }

    [Serializable]
    public class AssemblyOptions
    {
        public string Assembly { get; private set; }

        public AssemblyOptions(string assembly)
        {
            Assembly = assembly;
        }
    }

	public class TestRunOptions
	{
		private List<string> _tests = new List<string>();
        private List<string> _members = new List<string>();
        private List<string> _namespaces = new List<string>();

        public bool IsVerified { get; private set; }
        public IEnumerable<string> Tests { get { return _tests; } }
        public IEnumerable<string> Members { get { return _members; } }
        public IEnumerable<string> Namespaces { get { return _namespaces; } }

        public TestRunOptions HasBeenVerified(bool verified)
        {
            IsVerified = verified;
            return this;
        }

        public TestRunOptions AddTest(string test) {
			_tests.Add(test);
			return this;
		}

        public TestRunOptions AddTests(string[] tests) {
			_tests.AddRange(tests);
			return this;
		}
        public TestRunOptions AddMember(string member) { 
			_members.Add(member);
			return this;
		}

        public TestRunOptions AddMembers(string[] members) { 
			_members.AddRange(members);
			return this;
		}

        public TestRunOptions AddNamespace(string ns) { 
			_namespaces.Add(ns); 
			return this;
		}

        public TestRunOptions AddNamespaces(string[] namespaces) { 
			_namespaces.AddRange(namespaces);
			return this;
		}
	}
}
