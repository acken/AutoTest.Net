using System;
using System.Collections.Generic;
using System.Text;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Configuration;
using System.IO;
using AutoTest.Core.Caching.RunResultCache;
using EnvDTE80;
using EnvDTE;
using System.Drawing;

namespace AutoTest.VSAddin
{
    class FeedbackView : IRunFeedbackView
    {
        private IRunFeedbackPresenter _presenter;
        private InformationWindow _infoWindow;
        private IDirectoryWatcher _watcher;
        private IRunResultCache _resultsCache;
        private Action<RunMessages> _onMessage;
        private Action<IRunResultCache> _onCacheUpdate;
        private DTE2 _application;
        private FeedbackItemWindow _itemInfoWindow;

        public FeedbackView(Action<RunMessages> onMessage, Action<IRunResultCache> onCacheUpdate, DTE2 application)
        {
            _onMessage = onMessage;
            _onCacheUpdate = onCacheUpdate;
            _application = application;
            _presenter = BootStrapper.Services.Locate<IRunFeedbackPresenter>();
            _presenter.View = this;
            _resultsCache = BootStrapper.Services.Locate<IRunResultCache>();
            var infoPresenter = BootStrapper.Services.Locate<IInformationFeedbackPresenter>();
            _infoWindow = new InformationWindow(infoPresenter);
            _itemInfoWindow = new FeedbackItemWindow();
            _itemInfoWindow.LinkClicked += new EventHandler<StringArgs>(_itemInfoWindow_LinkClicked);
            BootStrapper.Services.Locate<IConfiguration>().ValidateSettings();
            _watcher = BootStrapper.Services.Locate<IDirectoryWatcher>();
            if (Connect.WatchFolder != null)
                _watcher.Watch(Connect.WatchFolder);
        }

        public void RecievingBuildMessage(BuildRunMessage message)
        {
            _onCacheUpdate.Invoke(_resultsCache);
        }

        public void RecievingRunFinishedMessage(RunFinishedMessage message)
        {
            var report = message.Report;
            var text = string.Format(
                        "Ran {0} build(s) ({1} succeeded, {2} failed) and {3} test(s) ({4} passed, {5} failed, {6} ignored)",
                        report.NumberOfProjectsBuilt,
                        report.NumberOfBuildsSucceeded,
                        report.NumberOfBuildsFailed,
                        report.NumberOfTestsRan,
                        report.NumberOfTestsPassed,
                        report.NumberOfTestsFailed,
                        report.NumberOfTestsIgnored);
            var runType = report.NumberOfBuildsFailed > 0 || report.NumberOfTestsFailed > 0 ? RunMessageType.Failed : RunMessageType.Succeeded;
            _onMessage.Invoke(new RunMessages(runType, text));
        }

        public void RecievingRunInformationMessage(RunInformationMessage message)
        {
            var text = "";
            switch (message.Type)
            {
                case InformationType.Build:
                    text = string.Format("building {0}", Path.GetFileName(message.Project));
                    break;
                case InformationType.TestRun:
                    text = string.Format("testing {0}", Path.GetFileName(message.Assembly));
                    break;
            }
            _onMessage.Invoke(new RunMessages(RunMessageType.Normal, text));
        }

        public void RecievingRunStartedMessage(RunStartedMessage message)
        {
            _onMessage.Invoke(new RunMessages(RunMessageType.Normal, "Detected changes, running.."));
        }

        public void RecievingTestRunMessage(TestRunMessage message)
        {
            _onCacheUpdate.Invoke(_resultsCache);
        }

        public void ShowInformation()
        {
            if (_infoWindow.Visible == false)
                _infoWindow.Visible = true;
            else
                _infoWindow.Show();
        }

        public void ShowMessageInfo(IItem item, Point mousePosition)
        {
            showBuildMessage(item.ToString(), mousePosition);
        }

        private void showBuildMessage(string text, Point mousePosition)
        {
            _itemInfoWindow.SetText(text, "Detailed run message information");
            mousePosition.Y -= _itemInfoWindow.Height;
            _itemInfoWindow.BringToFront(mousePosition);
        }

        void _itemInfoWindow_LinkClicked(object sender, StringArgs link)
        {
            goToReference(link.File, link.LineNumber);
        }

        public void GoToMessageReference(IItem item)
        {
            if (item.GetType().Equals(typeof(BuildItem)))
                goToBuildItemReference((BuildItem)item);
            if (item.GetType().Equals(typeof(TestItem)))
                goToTestItemReference((TestItem)item);
        }

        private void goToBuildItemReference(BuildItem buildItem)
        {
            goToReference(Path.Combine(Path.GetDirectoryName(buildItem.Key), buildItem.Value.File), buildItem.Value.LineNumber);
        }

        private void goToTestItemReference(TestItem testItem)
        {
            if (testItem.Value.StackTrace.Length > 0)
                goToReference(testItem.Value.StackTrace[0].File, testItem.Value.StackTrace[0].LineNumber);
        }

        private void goToReference(string file, int lineNumber)
        {
            try
            {
                var window = _application.OpenFile(EnvDTE.Constants.vsViewKindCode, file);
                window.Activate();
                var selection = (TextSelection)_application.ActiveDocument.Selection;
                selection.GotoLine(lineNumber, false);
            }
            catch (Exception ex)
            {
                var messag = ex.Message;
            }
        }
    }
}
