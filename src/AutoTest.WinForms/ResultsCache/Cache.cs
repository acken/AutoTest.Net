using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;

namespace AutoTest.WinForms.ResultsCache
{
    class Cache
    {
        private List<BuildItem> _errors = new List<BuildItem>();
        private List<BuildItem> _warnings = new List<BuildItem>();
        private List<TestItem> _failed = new List<TestItem>();
        private List<TestItem> _ignored = new List<TestItem>();

        public BuildItem[] Errors { get { return _errors.ToArray(); } }
        public BuildItem[] Warnings { get { return _warnings.ToArray(); } }
        public TestItem[] Failed { get { return _failed.ToArray(); } }
        public TestItem[] Ignored { get { return _ignored.ToArray(); } }

        public void Merge(BuildRunResults results)
        {
            mergeBuildList(_errors, results.Project, results.Errors);
            mergeBuildList(_warnings, results.Project, results.Warnings);
        }

        public void Merge(TestRunResults results)
        {
            mergeTestList(_failed, results.Assembly, results.Project, results.Failed);
            mergeTestList(_ignored, results.Assembly, results.Project, results.Ignored);
        }

        private void mergeBuildList(List<BuildItem> list, string key, BuildMessage[] results)
        {
            list.RemoveAll(e => e.Key.Equals(key));
            foreach (var message in results)
                list.Add(new BuildItem(key, message));
        }

        private void mergeTestList(List<TestItem> list, string key, string project, TestResult[] results)
        {
            list.RemoveAll(e => e.Key.Equals(key));
            foreach (var message in results)
                list.Add(new TestItem(key, project, message));
        }
    }
}
