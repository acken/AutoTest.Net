using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Core.Messaging;
using Rhino.Mocks;
using AutoTest.Messages;
using System.Threading;

namespace AutoTest.Test.Core.TestRunners.TestRunners
{
    [TestFixture]
    public class AutoTestTestRunnerTests
    {
        private IRunResultCache _cache;
        private IMessageBus _bus;
        private AutoTestRunnerFeedback _feedback;

        [SetUp]
        public void Setup()
        {
            _cache = MockRepository.GenerateMock<IRunResultCache>();
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _feedback = new AutoTestRunnerFeedback(_cache, _bus, new AutoTest.TestRunners.Shared.Options.RunOptions());
        }

        [Test]
        public void Should_publish_passing_test_that_has_previously_failed()
        {
            _cache.Stub(x => x.Failed).Return(new TestItem[] { new TestItem("", "", new Messages.TestResult(TestRunner.NUnit, TestRunStatus.Failed, "PreviouslyFailingTest")) });

            _feedback.TestFinished(new AutoTest.TestRunners.Shared.Results.TestResult("", "", "", 0, "PreviouslyFailingTest", AutoTest.TestRunners.Shared.Results.TestState.Passed, ""));

            _bus.AssertWasCalled(x => x.Publish<LiveTestStatusMessage>(null), x => x.IgnoreArguments());
        }

        [Test]
        public void Should_publish_failing_tests()
        {
            _cache.Stub(x => x.Failed).Return(new TestItem[] { });

            _feedback.TestFinished(new AutoTest.TestRunners.Shared.Results.TestResult("", "", "", 0, "Failing test", AutoTest.TestRunners.Shared.Results.TestState.Failed, ""));

            _bus.AssertWasCalled(x => x.Publish<LiveTestStatusMessage>(null), x => x.IgnoreArguments());
        }

        [Test]
        public void Should_publish_status_every_second()
        {
            _cache.Stub(x => x.Failed).Return(new TestItem[] { });

            var test = new AutoTest.TestRunners.Shared.Results.TestResult("", "", "", 0, "Passing test", AutoTest.TestRunners.Shared.Results.TestState.Passed, "");
            _feedback.TestFinished(test);
            _feedback.TestFinished(test);
            _feedback.TestFinished(test);
            _feedback.TestFinished(test);
            _feedback.TestFinished(test);
            _feedback.TestFinished(test);
            Thread.Sleep(1100);
            _feedback.TestFinished(test);

            _bus.AssertWasCalled(x => x.Publish<LiveTestStatusMessage>(null), x => x.IgnoreArguments());
        }
    }
}
