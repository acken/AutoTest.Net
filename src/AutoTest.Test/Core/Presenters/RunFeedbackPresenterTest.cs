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

namespace AutoTest.Test.Core.Presenters
{
    [TestFixture]
    public class RunFeedbackPresenterTest
    {
        private FakeRunFeedbackView _view;
        private RunFeedbackPresenter _presenter;
        private IMessageBus _bus;

        [SetUp]
        public void SetUp()
        {
            var locator = MockRepository.GenerateMock<IServiceLocator>();
            _bus = new MessageBus(locator);
            _view = new FakeRunFeedbackView();
            _presenter = new RunFeedbackPresenter(_bus);
            _presenter.View = _view;
        }

        [Test]
        public void Should_subscribe_to_build_messages()
        {
            var message = new BuildRunMessage(new BuildRunResults(""));
            _bus.Publish<BuildRunMessage>(message);
            waitForAsyncCall();
            _view.BuildRunMessage.ShouldBeTheSameAs(message);
        }

        [Test]
        public void Should_subscribe_to_testrun_messages()
        {
            var message = new TestRunMessage(new TestRunResults("", "", null));
            _bus.Publish(message);
            waitForAsyncCall();
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

        private void waitForAsyncCall()
        {
            Thread.Sleep(20);
        }
    }
}
