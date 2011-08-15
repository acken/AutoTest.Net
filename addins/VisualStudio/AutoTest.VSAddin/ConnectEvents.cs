using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using System.IO;
using AutoTest.Core.DebugLog;
using AutoTest.VSAddin.ATEngine;
using AutoTest.Messages;

namespace AutoTest.VSAddin
{
    public partial class Connect
    {
        private static SolutionEvents _solutionEvents = null;

        public static Window _toolWindow;
        public static FeedbackWindow _control;

        private static _dispSolutionEvents_OpenedEventHandler _openedEvent = null;
        private static _dispSolutionEvents_AfterClosingEventHandler _afterClosingEvent = null;

        public static string _WatchToken = null;
        public static Engine _engine = null;

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
            try
            {
                _applicationObject.ExecuteCommand("AutoTest.VSAddin.Connect.AutoTestNet_FeedbackWindow", "");
                _WatchToken = _applicationObject.Solution.FullName;
                _engine = new Engine(_control, _applicationObject);
                _engine.Bootstrap(_WatchToken);
                if (_engine.IsRunning)
                    _control.SetText("Engine is running and waiting for changes");
                else
                    _control.SetText("Engine is paused and will not detect changes");
            }
            catch (Exception ex)
            {
                Debug.WriteException(ex);
            }
        }

        private void onSolutionClosingFinished()
        {
            try
            {
                _engine.Shutdown();
            }
            catch (Exception ex)
            {
                Debug.WriteException(ex);
            }
        }
    }
}
