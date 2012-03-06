using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class RunInfoMerger
    {
        private List<RunInfo> _list;

        public RunInfoMerger(IEnumerable<RunInfo> list)
        {
            _list = list.ToList();
        }

        public List<RunInfo> MergeWith(List<RunInfo> newList)
        {
            newList.Where(x => x.ShouldBeBuilt && doesNotExist(x)).ToList()
                .ForEach(x => _list.Add(x));
            return _list;
        }

        private bool doesNotExist(RunInfo x)
        {
            return !_list.Exists(y => y.Project.Key.Equals(x.Project.Key));
        }
    }
}
