using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using System.Threading;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Targeting;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.Shared
{
    public class TestRunProcess
    {
        private static List<TestResult> _results = new List<TestResult>();
        private ITargetFrameworkLocator _locator;
        private ITestRunProcessFeedback _feedback = null;

        public static void AddResults(IEnumerable<TestResult> results)
        {
            lock (_results)
            {
                _results.AddRange(results);
            }
        }

        public TestRunProcess()
        {
            _locator = new TargetFrameworkLocator();
        }

        public TestRunProcess(ITestRunProcessFeedback feedback)
        {
            _locator = new TargetFrameworkLocator();
            _feedback = feedback;
        }

        public IEnumerable<TestResult> ProcessTestRuns(RunOptions options)
        {
            _results = new List<TestResult>();
            var workers = new List<Thread>();
            var testRuns = getTargetedRuns(options);
            foreach (var target in testRuns)
            {
                var process = new TestProcess(target, _feedback);
                var thread = new Thread(new ThreadStart(process.Start));
                thread.Start();
                workers.Add(thread);
            }
            foreach (var worker in workers)
                worker.Join();
            return _results;
        }

        private IEnumerable<TargetedRun> getTargetedRuns(RunOptions options)
        {
            var assembler = new TargetedRunAssembler(options, _locator);
            return assembler.Assemble();
        }
    }
}
