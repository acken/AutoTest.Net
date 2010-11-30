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

        public BuildItem[] Errors { get { lock (_padLock) { return _errors.ToArray(); } } }
        public BuildItem[] Warnings { get { lock (_padLock) { return _warnings.ToArray(); } } }
        public TestItem[] Failed { get { lock (_padLock) { return _failed.ToArray(); } } }
        public TestItem[] Ignored { get { lock (_padLock) { return _ignored.ToArray(); } } }

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

        private void mergeBuildList(List<BuildItem> list, string key, BuildMessage[] results)
        {
            list.RemoveAll(e => e.Key.Equals(key));
            foreach (var message in results)
                list.Insert(0, new BuildItem(key, message));
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
