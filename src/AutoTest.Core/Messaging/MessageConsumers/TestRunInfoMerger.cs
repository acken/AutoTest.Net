using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    class TestRunInfoMerger
    {
        private List<RunInfo> _list;

        public TestRunInfoMerger(IEnumerable<RunInfo> list)
        {
            _list = list.ToList();
        }

        public List<RunInfo> MergeWith(List<RunInfo> list)
        {
            addUnexisting(list);
            mergeExisting(list);
            return _list;
        }

        private void mergeExisting(List<RunInfo> list)
        {
            var tests = new List<KeyValuePair<RunInfo, TestToRun>>();
            addNewEntries(list, (info) => { return info.GetTests().ToList(); }, (info, test) => { info.AddTestsToRun(new TestToRun[] { test }); });
            addNewEntries(list, (info) => { return info.GetMembers().ToList(); }, (info, member) => { info.AddMembersToRun(new TestToRun[] { member }); });
            addNewEntries(list, (info) => { return info.GetNamespaces().ToList(); }, (info, ns) => { info.AddNamespacesToRun(new TestToRun[] { ns }); });
        }

        public void addNewEntries(List<RunInfo> list, Func<RunInfo, List<TestToRun>> getItems, Action<RunInfo, TestToRun> addItem)
        {
            list.Where(x => exists(x)).ToList().ForEach(x => getItems.Invoke(x).ForEach(t => addIfNew(x, t, getItems, addItem)));
        }

        private void addIfNew(RunInfo x, TestToRun t, Func<RunInfo, List<TestToRun>> getItems, Action<RunInfo, TestToRun> addItem)
        {
            var current = _list.Where(y => y.Assembly.Equals(x.Assembly)).First();
            if (!testExists(t, getItems, current))
                addItem(current, t);
        }

        private static bool testExists(TestToRun t, Func<RunInfo, List<TestToRun>> getItems, RunInfo current)
        {
            return getItems(current).Exists(test => compareTests(t, test));
        }

        private static bool compareTests(TestToRun a, TestToRun b)
        {
            return a.Runner.Equals(b.Runner) && a.Test.Equals(b.Test);
        }

        private void addUnexisting(List<RunInfo> list)
        {
            list.Where(x => !exists(x)).ToList()
                .ForEach(x => _list.Add(x));
        }

        private bool exists(RunInfo x)
        {
            return _list.Exists(y => y.Assembly.Equals(x.Assembly));
        }
    }
}
