using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;

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
            }
        }

        void _bus_OnRunStartedMessage(object sender, RunStartedMessageEventArgs e)
        {
            _view.RecievingRunStartedMessage(e.Message);
        }

        void _bus_OnRunFinishedMessage(object sender, RunFinishedMessageEventArgs e)
        {
            _view.RecievingRunFinishedMessage(e.Message);
        }

        public RunFeedbackPresenter(IMessageBus bus)
        {
            _bus = bus;
        }

        void  _bus_OnBuildMessage(object sender, BuildMessageEventArgs e)
        {
 	        _view.RecievingBuildMessage(e.Message);
        }

        void _bus_OnTestRunMessage(object sender, TestRunMessageEventArgs e)
        {
            _view.RecievingTestRunMessage(e.Message);
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
