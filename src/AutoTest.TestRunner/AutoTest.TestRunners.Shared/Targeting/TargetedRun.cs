using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;

namespace AutoTest.TestRunners.Shared.Targeting
{
    class TargetedRun
    {
        private List<RunnerOptions> _runOptions = new List<RunnerOptions>();

        public Version TargetFramework { get; private set; }
        public IEnumerable<RunnerOptions> Runners { get { return _runOptions; } }

        public TargetedRun(Version targetFramework)
        {
            TargetFramework = targetFramework;
        }

        public void AddRunner(string runnerID, IEnumerable<string> categories, AssemblyOptions assemblyOptions)
        {
            var option = _runOptions.Where(x => x.ID.Equals(runnerID)).FirstOrDefault();
            if (option == null)
                option = addOption(runnerID);
            option.AddAssembly(assemblyOptions);
            mergeCategories(option, categories);
        }

        private void mergeCategories(RunnerOptions option, IEnumerable<string> categories)
        {
            var newCategories = categories.Where(x => !option.Categories.Contains(x)).Select(x => x);
            option.AddCategories(newCategories.ToArray());
        }

        private RunnerOptions addOption(string runnerID)
        {
            _runOptions.Add(new RunnerOptions(runnerID));
            return _runOptions[_runOptions.Count -1];
        }
    }
}
