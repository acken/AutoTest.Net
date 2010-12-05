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
        private bool _deltasSupported = false;

        private List<BuildItem> _errors = new List<BuildItem>();
        private List<BuildItem> _warnings = new List<BuildItem>();
        private List<TestItem> _failed = new List<TestItem>();
        private List<TestItem> _ignored = new List<TestItem>();

        private List<BuildItem> _lastErrors = new List<BuildItem>();
        private List<BuildItem> _lastWarnings = new List<BuildItem>();
        private List<TestItem> _lastFailed = new List<TestItem>();
        private List<TestItem> _lastIgnored = new List<TestItem>();

        public BuildItem[] Errors { get { return _errors.ToArray(); } }
        public BuildItem[] Warnings { get { return _warnings.ToArray(); } }
        public TestItem[] Failed { get { return _failed.ToArray(); } }
        public TestItem[] Ignored { get { return _ignored.ToArray(); } }

        public void Merge(BuildRunResults results)
        {
            lock (_padLock)
            {
                mergeBuildList(_errors, results.Project, results.Errors);
                mergeBuildList(_warnings, results.Project, results.Warnings);
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

        public void EnabledDeltas()
        {
            _deltasSupported = true;
        }

        public RunResultCacheDeltas PopDeltas()
        {
            if (!_deltasSupported)
                throw new Exception("Deltas are not supported in the run results cache. Run EnabledDeltas() to setup support");
            lock (_padLock)
            {
                var deltas = new RunResultCacheDeltas();
                deltas.Load(_lastErrors, _lastWarnings, _lastFailed, _lastIgnored, _errors, _warnings, _failed, _ignored);
                _lastErrors.Clear();
                _lastWarnings.Clear();
                _lastFailed.Clear();
                _lastIgnored.Clear();

                _lastErrors.AddRange(_errors);
                _lastWarnings.AddRange(_warnings);
                _lastFailed.AddRange(_failed);
                _lastIgnored.AddRange(_ignored);
                return deltas;
            }
        }

        private void mergeBuildList(List<BuildItem> list, string key, BuildMessage[] results)
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
            foreach (var item in itemsToRemove)
                list.Remove(item);
            foreach (var message in results)
            {
                var item = new BuildItem(key, message);
                if (!list.Contains(item))
                {
                    list.Insert(0, item);
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
                list.RemoveAll(i => i.IsTheSameTestAs(item));
        }

        private void moveTestsBetweenStates(TestRunResults results, TestResult[] newSstate, List<TestItem> oldState)
        {
            foreach (var test in newSstate)
            {
                var item = new TestItem(results.Assembly, results.Project, test);
                if (oldState.Exists(i => i.IsTheSameTestAs(item)))
                    oldState.RemoveAll(i => i.IsTheSameTestAs(item));
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
                        list.RemoveAll(i => i.IsTheSameTestAs(item));
                    list.Insert(0, item);
                }
            }
        }
    }
}
