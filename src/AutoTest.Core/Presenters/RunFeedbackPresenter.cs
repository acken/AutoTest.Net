using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;

namespace AutoTest.Core.Presenters
{
    class RunFeedbackPresenter : IRunFeedbackPresenter
    {
        private IMessageBus _bus;
        private IRunFeedbackView _view;

        public IRunFeedbackView View
        {
            get { return _view; }
            set
            {
                _view = value;
                _bus.OnBuildMessage +=new EventHandler<BuildMessageEventArgs>(_bus_OnBuildMessage);
                _bus.OnTestRunMessage += new EventHandler<TestRunMessageEventArgs>(_bus_OnTestRunMessage);
            }
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
    }
}
