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

        private List<TestItem> _addedTests = new List<TestItem>();
        private List<TestItem> _removedTests = new List<TestItem>();

        public BuildItem[] Errors { get { return _errors.ToArray(); } }
        public BuildItem[] Warnings { get { return _warnings.ToArray(); } }
        public TestItem[] Failed { get { return _failed.ToArray(); } }
        public TestItem[] Ignored { get { return _ignored.ToArray(); } }

        public BuildItem[] AddedErrors { get { return _addedErrors.ToArray(); } }
        public BuildItem[] RemovedErrors { get { return _removedErrors.ToArray(); } }
        public BuildItem[] AddedWarnings { get { return _addedWarnings.ToArray(); } }
        public BuildItem[] RemovedWarnings { get { return _removedWarnings.ToArray(); } }

        public TestItem[] AddedTests { get { return _addedTests.ToArray(); } }
        public TestItem[] RemovedTests { get { return _removedTests.ToArray(); } }

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
                _addedTests.Clear();
                _removedTests.Clear();
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
                var item = new TestItem(results.Assembly, results.Project, test);
                removeIfExists(item, _ignored);
                removeIfExists(item, _failed);
            }
            moveTestsBetweenStates(results, results.Failed, _ignored);
            moveTestsBetweenStates(results, results.Ignored, _failed);
        }

        private void removeIfExists(TestItem item, List<TestItem> list)
        {
            if (list.Exists(i => i.IsTheSameTestAs(item)))
            {
                list.RemoveAll(i => i.IsTheSameTestAs(item));
                _removedTests.Add(item);
            }
        }

        private void moveTestsBetweenStates(TestRunResults results, TestResult[] newSstate, List<TestItem> oldState)
        {
            foreach (var test in newSstate)
            {
                var item = new TestItem(results.Assembly, results.Project, test);
                if (oldState.Exists(i => i.IsTheSameTestAs(item)))
                {
                    oldState.RemoveAll(i => i.IsTheSameTestAs(item));
                    _removedTests.Add(item);
                }
            }
        }

        private void mergeTestList(List<TestItem> list, string key, string project, TestResult[] results, TestResult[] passingTests)
        {
            foreach (var test in results)
            {
                var item = new TestItem(key, project, test);
                if (!list.Contains(item))
                {
                    if (list.Exists(i => i.IsTheSameTestAs(item)))
                    {
                        list.RemoveAll(i => i.IsTheSameTestAs(item));
                        _removedTests.Add(item);
                    }
                    list.Insert(0, item);
                    _addedTests.Insert(0, item);
                }
            }
        }
    }
}
