using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching.RunResultCache
{
    public class RunResultCacheDeltas
    {
        private List<BuildItem> _addedErrors = new List<BuildItem>();
        private List<BuildItem> _removedErrors = new List<BuildItem>();
        private List<BuildItem> _addedWarnings = new List<BuildItem>();
        private List<BuildItem> _removedWarnings = new List<BuildItem>();

        private List<TestItem> _addedTests = new List<TestItem>();
        private List<TestItem> _removedTests = new List<TestItem>();

        public BuildItem[] AddedErrors { get { return _addedErrors.ToArray(); } }
        public BuildItem[] RemovedErrors { get { return _removedErrors.ToArray(); } }
        public BuildItem[] AddedWarnings { get { return _addedWarnings.ToArray(); } }
        public BuildItem[] RemovedWarnings { get { return _removedWarnings.ToArray(); } }

        public TestItem[] AddedTests { get { return _addedTests.ToArray(); } }
        public TestItem[] RemovedTests { get { return _removedTests.ToArray(); } }

        public void AddError(BuildItem error) { _addedErrors.Add(error); }
        public void RemoveError(BuildItem error) { _removedErrors.Add(error); }
        public void AddWarning(BuildItem warning) { _addedWarnings.Add(warning); }
        public void RemoveWarning(BuildItem warning) { _removedWarnings.Add(warning); }
        public void AddTest(TestItem test) { _addedTests.Add(test); }
        public void RemoveTest(TestItem ignored) { _removedTests.Add(ignored); }

        public void Load(List<BuildItem> lastErrors, List<BuildItem> lastWarnings, List<TestItem> lastFailed, List<TestItem> lastIgnored, List<BuildItem> errors, List<BuildItem> warnings, List<TestItem> failed, List<TestItem> ignored)
        {
            getBuildDeltas(lastErrors, errors, _addedErrors, _removedErrors);
            getBuildDeltas(lastWarnings, warnings, _addedWarnings, _removedWarnings);
            getTestDeltas(lastFailed, failed, _addedTests, _removedTests);
            getTestDeltas(lastIgnored, ignored, _addedTests, _removedTests);
        }

        private void getBuildDeltas(List<BuildItem> lastBuildItems, List<BuildItem> buildItems, List<BuildItem> added, List<BuildItem> removed)
        {
            foreach (var error in buildItems)
            {
                if (!lastBuildItems.Contains(error))
                    added.Add(error);
            }
            foreach (var error in lastBuildItems)
            {
                if (!buildItems.Contains(error))
                    removed.Add(error);
            }
        }

        private void getTestDeltas(List<TestItem> lastTests, List<TestItem> tests, List<TestItem> added, List<TestItem> removed)
        {
            foreach (var test in tests)
            {
                if (!lastTests.Contains(test))
                    added.Add(test);
            }
            foreach (var test in lastTests)
            {
                if (!tests.Contains(test))
                    removed.Add(test);
            }
        }
    }
}
