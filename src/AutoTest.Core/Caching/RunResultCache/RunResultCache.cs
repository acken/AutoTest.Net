using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using AutoTest.Messages;
using AutoTest.Core.DebugLog;

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
                Debug.WriteDebug("Merging build run results");
                mergeBuildList(_errors, results.Project, results.Errors);
                mergeBuildList(_warnings, results.Project, results.Warnings);
            }
        }

        public void Merge(TestRunResults results)
        {
            lock (_padLock)
            {
                Debug.WriteDebug("Merging test run results");
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
                {
                    Debug.WriteDebug("Removing old build item from {0} in {1}, {2} {3}:{4}", item.Key, item.Value.File, item.Value.ErrorMessage, item.Value.LineNumber, item.Value.LinePosition);
                    itemsToRemove.Add(item);
                }
            }
            foreach (var item in itemsToRemove)
                list.Remove(item);
            foreach (var message in results)
            {
                var item = new BuildItem(key, message);
                if (!list.Contains(item))
                {
                    Debug.WriteDebug("Adding new build item from {0} in {1}, {2} {3}:{4}", item.Key, item.Value.File, item.Value.ErrorMessage, item.Value.LineNumber, item.Value.LinePosition);
                    list.Insert(0, item);
                }
            }
        }

        private void removeChanged(TestRunResults results)
        {
            _failed.RemoveAll(x => x.Value.Runner == TestRunner.Any);
            _ignored.RemoveAll(x => x.Value.Runner == TestRunner.Any);
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
                logTest("Removing passing test ", item);
                list.RemoveAll(i => i.IsTheSameTestAs(item));
            }
        }

        private void moveTestsBetweenStates(TestRunResults results, TestResult[] newSstate, List<TestItem> oldState)
        {
            foreach (var test in newSstate)
            {
                var item = new TestItem(results.Assembly, results.Project, test);
                if (oldState.Exists(i => i.IsTheSameTestAs(item)))
                {
                    logTest("Removing test that changed state ", item);
                    oldState.RemoveAll(i => i.IsTheSameTestAs(item));
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
                        logTest("Removing existing test in case it changed ", item);
                        list.RemoveAll(i => i.IsTheSameTestAs(item));
                    }
                    logTest("Adding test ", item);
                    list.Insert(0, item);
                }
            }
        }

        private void logTest(string prefix, TestItem item)
        {
            Debug.WriteDebug("{6} ({2}.{1}) from {0} named {4} saying {3} in {5}", item.Key, item.Value.Status, item.Value.Runner, item.Value.Name, item.Value.Message, getStackTrace(item.Value.StackTrace), prefix);
        }

        private string getStackTrace(IStackLine[] iStackLine)
        {
            var builder = new StringBuilder();
            foreach (var line in iStackLine)
                builder.Append(string.Format(" {0}, {1}:{2}", line.File, line.Method, line.LineNumber));
            return builder.ToString();
        }
    }
}
