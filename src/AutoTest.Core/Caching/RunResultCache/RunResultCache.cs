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
                mergeTestList(_failed, results.Assembly, results.Project, results.Failed, results.Passed, results.IsPartialTestRun);
                mergeTestList(_ignored, results.Assembly, results.Project, results.Ignored, results.Passed, results.IsPartialTestRun);
            }
        }

        private void mergeBuildList(List<BuildItem> list, string key, BuildMessage[] results)
        {
            list.RemoveAll(e => e.Key.Equals(key));
            foreach (var message in results)
                list.Insert(0, new BuildItem(key, message));
        }

        private void mergeTestList(List<TestItem> list, string key, string project, TestResult[] results, TestResult[] passingTests, bool isPartial)
        {
            if (isPartial)
                mergePartialRunTestList(list, key, project, results, passingTests);
            else
                mergeFullRunTestList(list, key, project, results);
        }

        private void mergePartialRunTestList(List<TestItem> list, string key, string project, TestResult[] results, TestResult[] passingTests)
        {
            foreach (var test in passingTests)
                list.RemoveAll(e => e.Value.Name.Equals(test.Name));
            foreach (var test in results)
            {
                if (!list.Exists(e => e.Value.Name.Equals(test.Name)))
                    list.Insert(0, new TestItem(key, project, test));
            }
        }

        private void mergeFullRunTestList(List<TestItem> list, string key, string project, TestResult[] results)
        {
            list.RemoveAll(e => e.Key.Equals(key));
            foreach (var message in results)
                list.Insert(0, new TestItem(key, project, message));
        }
    }
}
