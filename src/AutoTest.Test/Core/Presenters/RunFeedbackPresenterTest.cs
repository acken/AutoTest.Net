using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using NUnit.Framework;
using AutoTest.Core.Configuration;
using Rhino.Mocks;
using AutoTest.Core.Messaging;
using AutoTest.Core.Presenters;
using AutoTest.Test.Core.Presenters.Fakes;
using System.Threading;
using AutoTest.Core.Caching;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Presenters
{
    [TestFixture]
    public class RunFeedbackPresenterTest
    {
        private FakeRunFeedbackView _view;
        private RunFeedbackPresenter _presenter;
        private IMessageBus _bus;
        private FakeRunResultMerger _resultMerger;

        [SetUp]
        public void SetUp()
        {
            var locator = MockRepository.GenerateMock<IServiceLocator>();
            _bus = new MessageBus(locator);
            _resultMerger = new FakeRunResultMerger();
            _view = new FakeRunFeedbackView();
            _presenter = new RunFeedbackPresenter(_bus, _resultMerger);
            _presenter.View = _view;
        }

        [Test]
        public void Should_subscribe_to_build_messages()
        {
            var message = new BuildRunMessage(new BuildRunResults(""));
            _bus.Publish<BuildRunMessage>(message);
            waitForAsyncCall();
            _resultMerger.HasMergedBuildResults.ShouldBeTrue();
            _view.BuildRunMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_testrun_messages()
        {
            var message = new TestRunMessage(new TestRunResults("", "", false, null));
            _bus.Publish(message);
            waitForAsyncCall();
            _resultMerger.HasMergedTestResults.ShouldBeTrue();
            _view.TestRunMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_run_started_messages()
        {
            var message = new RunStartedMessage(null);
            _bus.Publish(message);
            waitForAsyncCall();
            _view.RunStartedMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_run_finished_messages()
        {
            var message = new RunFinishedMessage(null);
            _bus.Publish(message);
            waitForAsyncCall();
            _view.RunFinishedMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_run_information_messages()
        {
            var message = new RunInformationMessage(InformationType.Build, "", "", "".GetType());
            _bus.Publish(message);
            waitForAsyncCall();
            _view.RunInformationMessage.ShouldBeTheSameAs(message);
        }

        private void waitForAsyncCall()
        {
            Thread.Sleep(30);
        }
    }
}
