using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using AutoTest.Core.Caching.RunResultCache;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    class RemovedTestsLocator : ILocateRemovedTests
    {
        private IRunResultCache _cache;
        private TestRunInfo[] _infos;
        private TestRunResults _results;

        public RemovedTestsLocator(IRunResultCache cache)
        {
            _cache = cache;
        }

        public TestRunResults SetRemovedTestsAsPassed(TestRunResults results, TestRunInfo[] infos)
        {
            _results = results;
            _infos = infos;
            var tests = new List<TestResult>();
            tests.AddRange(results.All);
            tests.AddRange(getTests(_cache.Failed));
            tests.AddRange(getTests(_cache.Ignored));
            var modified = new TestRunResults(_results.Project, _results.Assembly, _results.IsPartialTestRun, _results.Runner, tests.ToArray());
            modified.SetTimeSpent(_results.TimeSpent);
            return modified;
        }

        private TestResult[] getTests(TestItem[] cacheList)
        {
            var tests = new List<TestResult>();
            foreach (var test in cacheList)
            {
                if (!test.Value.Runner.Equals(_results.Runner))
                    continue;
                if ((from t in _results.All where new TestItem(_results.Assembly, _results.Project, t).IsTheSameTestAs(test) select t).Count() == 0)
                {
                    if (_results.IsPartialTestRun && !wasRun(test))
                        continue;
                    tests.Add(new TestResult(test.Value.Runner, TestRunStatus.Passed, test.Value.Name));
                }
            }
            return tests.ToArray();
        }

        private bool wasRun(TestItem test)
        {
            foreach (var info in _infos)
            {
                if (info.Assembly.Equals(_results.Assembly))
                {
                    if (info.GetTestsFor(test.Value.Runner).Contains(test.Value.Name))
                        return true;
                }
            }
            return false;
        }
    }
}
