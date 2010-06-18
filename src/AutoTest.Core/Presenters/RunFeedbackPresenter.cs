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
            }
        }

        public RunFeedbackPresenter(IMessageBus bus)
        {
            _bus = bus;
        }

        void  _bus_OnBuildMessage(object sender, BuildMessageEventArgs e)
        {
 	        _view.RecievingBuildMessage(e.RunMessage);
        }
    }
}
