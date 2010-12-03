using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using AutoTest.Messages;

namespace AutoTest.Core.Caching.RunResultCache
{
    class RunResultCache : IRunResultCache, IMergeRunResults
    {
        private object _padLock = new object();

        private List<BuildItem> _errors = new List<BuildItem>();
        private List<BuildItem> _warnings = new List<BuildItem>();
        private List<TestItem> _failed = new List<TestItem>();
        private List<TestItem> _ignored = new List<TestItem>();

        private List<BuildItem> _addedErrors = new List<BuildItem>();
        private List<BuildItem> _removedErrors = new List<BuildItem>();
        private List<BuildItem> _addedWarnings = new List<BuildItem>();
        private List<BuildItem> _removedWarnings = new List<BuildItem>();

        public BuildItem[] Errors { get { return _errors.ToArray(); } }
        public BuildItem[] Warnings { get { return _warnings.ToArray(); } }
        public TestItem[] Failed { get { return _failed.ToArray(); } }
        public TestItem[] Ignored { get { return _ignored.ToArray(); } }

        public BuildItem[] AddedErrors { get { return _addedErrors.ToArray(); } }
        public BuildItem[] RemovedErrors { get { return _removedErrors.ToArray(); } }
        public BuildItem[] AddedWarnings { get { return _addedWarnings.ToArray(); } }
        public BuildItem[] RemovedWarnings { get { return _removedWarnings.ToArray(); } }

        public void Merge(BuildRunResults results)
        {
            lock (_padLock)
            {
                _addedErrors.Clear();
                _removedErrors.Clear();
                _addedWarnings.Clear();
                _removedWarnings.Clear();
                mergeBuildList(_errors, results.Project, results.Errors, _addedErrors, _removedErrors);
                mergeBuildList(_warnings, results.Project, results.Warnings, _addedWarnings, _removedWarnings);
            }
        }

        public void Merge(TestRunResults results)
        {
            lock (_padLock)
            {
                removeChanged(results);
                mergeTestList(_failed, results.Assembly, results.Project, results.Failed, results.Passed);
                mergeTestList(_ignored, results.Assembly, results.Project, results.Ignored, results.Passed);
            }
        }

        private void mergeBuildList(List<BuildItem> list, string key, BuildMessage[] results, List<BuildItem> added, List<BuildItem> removed)
        {
            var itemsToRemove = new List<BuildItem>();
            foreach (var item in list)
            {
                var found = false;
                foreach (var message in results)
                {
                    var resultItem = new BuildItem(key, message);
                    if (resultItem.Equals(item))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found && item.Key.Equals(key))
                    itemsToRemove.Add(item);
            }
            removed.AddRange(itemsToRemove.ToArray());
            foreach (var item in itemsToRemove)
                list.Remove(item);
            foreach (var message in results)
            {
                var item = new BuildItem(key, message);
                if (!list.Contains(item))
                {
                    list.Insert(0, item);
                    added.Insert(0, item);
                }
            }
        }

        private void removeChanged(TestRunResults results)
        {
            foreach (var test in results.Passed)
            {
                _ignored.RemoveAll(e => compareTests(test, e, results.Assembly));
                _failed.RemoveAll(e => compareTests(test, e, results.Assembly));
            }
            foreach (var test in results.Failed)
                _ignored.RemoveAll(e => compareTests(test, e, results.Assembly));
            foreach (var test in results.Ignored)
                _failed.RemoveAll(e => compareTests(test, e, results.Assembly));
        }

        private void mergeTestList(List<TestItem> list, string key, string project, TestResult[] results, TestResult[] passingTests)
        {
            foreach (var test in results)
            {
                if (!list.Exists(e => compareTests(test, e, key)))
                    list.Insert(0, new TestItem(key, project, test));
            }
        }

        private bool compareTests(TestResult test, TestItem e, string assembly)
        {
            return e.Key.Equals(assembly) && e.Value.Name.Equals(test.Name) && e.Value.Runner.Equals(test.Runner);
        }
    }
}
