using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Presenters
{
    class RunFeedbackPresenter : IRunFeedbackPresenter, IDisposable
    {
        private IMessageBus _bus;
        private IRunFeedbackView _view;

        public IRunFeedbackView View
        {
            get { return _view; }
            set
            {
                _view = value;
                _bus.OnRunStartedMessage += new EventHandler<RunStartedMessageEventArgs>(_bus_OnRunStartedMessage);
                _bus.OnRunFinishedMessage += new EventHandler<RunFinishedMessageEventArgs>(_bus_OnRunFinishedMessage);
                _bus.OnBuildMessage +=new EventHandler<BuildMessageEventArgs>(_bus_OnBuildMessage);
                _bus.OnTestRunMessage += new EventHandler<TestRunMessageEventArgs>(_bus_OnTestRunMessage);
                _bus.OnRunInformationMessage += new EventHandler<RunInformationMessageEventArgs>(_bus_OnRunInformationMessage);
            }
        }

        public RunFeedbackPresenter(IMessageBus bus)
        {
            _bus = bus;
        }

        void _bus_OnRunStartedMessage(object sender, RunStartedMessageEventArgs e)
        {
            Debug.PresenterRecievedRunStartedMessage();
            _view.RecievingRunStartedMessage(e.Message);
        }

        void _bus_OnRunFinishedMessage(object sender, RunFinishedMessageEventArgs e)
        {
            Debug.PresenterRecievedRunFinishedMessage();
            _view.RecievingRunFinishedMessage(e.Message);
        }

        void  _bus_OnBuildMessage(object sender, BuildMessageEventArgs e)
        {
            Debug.PresenterRecievedBuildMessage();
 	        _view.RecievingBuildMessage(e.Message);
        }

        void _bus_OnTestRunMessage(object sender, TestRunMessageEventArgs e)
        {
            Debug.PresenterRecievedTestRunMessage();
            _view.RecievingTestRunMessage(e.Message);
        }

        void _bus_OnRunInformationMessage(object sender, RunInformationMessageEventArgs e)
        {
            Debug.PresenterRecievedRunInformationMessage();
            _view.RecievingRunInformationMessage(e.Message);
        }

        #region IDisposable Members

        public void Dispose()
        {
            _bus.OnRunStartedMessage -= _bus_OnRunStartedMessage;
            _bus.OnRunFinishedMessage -= _bus_OnRunFinishedMessage;
            _bus.OnBuildMessage -= _bus_OnBuildMessage;
            _bus.OnTestRunMessage -= _bus_OnTestRunMessage;
        }

        #endregion
    }
}
