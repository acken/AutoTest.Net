using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;

namespace AutoTest.VSAddin.Listeners
{
    class FeedbackListener : IMessageForwarder
    {
        private FeedbackWindow _window;

        public FeedbackListener(FeedbackWindow window)
        {
            _window = window;
        }

        public void Forward(object message)
        {
            _window.Consume(message);
        }
    }
}
