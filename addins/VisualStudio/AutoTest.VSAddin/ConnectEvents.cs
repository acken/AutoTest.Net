using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using System.IO;

namespace AutoTest.VSAddin
{
    public partial class Connect
    {
        private static SolutionEvents _solutionEvents = null;

        private Window _toolWindow;
        private FeedbackWindow _control;

        private static _dispSolutionEvents_OpenedEventHandler _openedEvent = null;
        private static _dispSolutionEvents_AfterClosingEventHandler _afterClosingEvent = null;

        public static string WatchFolder = null;

        private void bindWorkspaceEvents()
        {
            bindSolutionEvents();
            bindEventsOnSolution();
        }

        private void bindSolutionEvents()
        {
            if (_solutionEvents == null)
                _solutionEvents = _applicationObject.Events.SolutionEvents;
        }

        private void bindEventsOnSolution()
        {
            if (_openedEvent != null)
                return;
            
            _openedEvent = new _dispSolutionEvents_OpenedEventHandler(onSolutionOpened);
            _solutionEvents.Opened += _openedEvent;

            _afterClosingEvent = new _dispSolutionEvents_AfterClosingEventHandler(onSolutionClosingFinished);
            _solutionEvents.AfterClosing += _afterClosingEvent;
        }

        private void onSolutionOpened()
        {
            if (!File.Exists(_applicationObject.Solution.FullName))
                return;
            WatchFolder = Path.GetDirectoryName(_applicationObject.Solution.FullName);
            bootStrapAutoTest(WatchFolder);
        }

        private void onSolutionClosingFinished()
        {
            terminateAutoTest();
        }
    }
}
