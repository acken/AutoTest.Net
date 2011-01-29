using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.Shared.Targeting
{
    class TargetedRunAssembler
    {
        private RunOptions _options;
        private IAssemblyParser _locator;
        private List<TargetedRun> _runs;

        public TargetedRunAssembler(RunOptions options, IAssemblyParser locator)
        {
            _options = options;
            _locator = locator;
        }

        public IEnumerable<TargetedRun> Assemble()
        {
            _runs = new List<TargetedRun>();
            foreach (var run in _options.TestRuns)
            {
                foreach (var assembly in run.Assemblies)
                    addAssembly(run.ID, run.Categories, assembly);
            }
            return _runs;
        }

        private void addAssembly(string runnerID, IEnumerable<string> categories, AssemblyOptions assemblyOptions)
        {
            var targetFramework = getTargetFramework(assemblyOptions.Assembly);
            var targeted = _runs.Where(x => x.TargetFramework.Equals(targetFramework)).FirstOrDefault();
            if (targeted == null)
                targeted = addTarget(targetFramework);
            targeted.AddRunner(runnerID, categories, assemblyOptions);
        }

        private TargetedRun addTarget(Version targetFramework)
        {
            _runs.Add(new TargetedRun(targetFramework));
            return _runs[_runs.Count -1];
        }

        private Version getTargetFramework(string assembly)
        {
            return _locator.GetTargetFramework(assembly);
        }
    }
}
