using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Presenters;
using AutoTest.Test.Core.Presenters.Fakes;
using AutoTest.Core.Messaging;
using AutoTest.Test.Core.Caching.Projects.Fakes;
using Rhino.Mocks;
using AutoTest.Core.Configuration;
using System.Threading;

namespace AutoTest.Test.Core.Presenters
{
    [TestFixture]
    public class FeedbackPresenterTest
    {
        private FakeIFeedbackView _view;
        private FeedbackPresenter _presenter;
        private IMessageBus _bus;

        [SetUp]
        public void SetUp()
        {
            var locator = MockRepository.GenerateMock<IServiceLocator>();
            _bus = new MessageBus(locator);
            _view = new FakeIFeedbackView();
            _presenter = new FeedbackPresenter(_bus);
            _presenter.View = _view;
        }

        [Test]
        public void Should_subscribe_to_information_messages()
        {
            _bus.Publish<InformationMessage>(new InformationMessage("some value"));
            waitForAsyncCall();
            _view.Message.ShouldEqual("some value");
        }

        private void waitForAsyncCall()
        {
            Thread.Sleep(20);
        }
    }
}
