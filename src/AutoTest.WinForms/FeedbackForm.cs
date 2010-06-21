using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Configuration;
using System.Threading;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using AutoTest.WinForms.ResultsCache;

namespace AutoTest.WinForms
{
    public partial class FeedbackForm : Form, IOverviewForm, IRunFeedbackView
    {
        private SynchronizationContext _syncContext;
        private IRunFeedbackPresenter _runPresenter;
        private IDirectoryWatcher _watcher;
        private IConfiguration _configuration;
        private Cache _cache = new Cache();

        public FeedbackForm(IDirectoryWatcher watcher, IConfiguration configuration, IRunFeedbackPresenter runPresenter)
        {
            _syncContext = AsyncOperationManager.SynchronizationContext;
            _watcher = watcher;
            _configuration = configuration;
            _runPresenter = runPresenter;
            _runPresenter.View = this;
            InitializeComponent();
            _watcher.Watch(configuration.DirectoryToWatch);
        }

        #region IOverviewForm Members

        public Form Form
        {
            get { return this; }
        }

        #endregion
    
        #region IRunFeedbackView Members

        public void  RecievingRunStartedMessage(RunStartedMessage message)
        {

            _syncContext.Post(s =>
                                  {
                                      labelRunState.Text = "Detected changes, running..";
                                      labelRunState.ForeColor = Color.Black;
                                  }, null);
        }

        public void  RecievingRunFinishedMessage(RunFinishedMessage message)
        {
            _syncContext.Post(
                x =>
                {
                    var report = (RunReport) x;
                    labelRunState.Text = string.Format(
                        "Ran {0} build(s) ({1} succeeded, {2} failed) and {3} test(s) ({4} passed, {5} failed, {6} ignored)",
                        report.NumberOfProjectsBuilt,
                        report.NumberOfBuildsSucceeded,
                        report.NumberOfBuildsFailed,
                        report.NumberOfTestsRan,
                        report.NumberOfTestsPassed,
                        report.NumberOfTestsFailed,
                        report.NumberOfTestsIgnored);
                    labelRunState.ForeColor =
                        report.NumberOfBuildsFailed > 0 ||
                        report.NumberOfTestsFailed > 0 ||
                        report.NumberOfTestsIgnored > 0
                            ? Color.Red
                            : Color.Green;
                },
                message.Report);
        }

        public void  RecievingBuildMessage(BuildRunMessage message)
        {
            _syncContext.Post(m =>
                                  {
                                      var results = (BuildRunResults) m;
                                      _cache.Merge(results);
                                      relistFromCache();
                                  },
                              message.Results);
        }

        public void  RecievingTestRunMessage(TestRunMessage message)
        {
            _syncContext.Post(m =>
                                  {
                                      var results = (TestRunResults) m;
                                      _cache.Merge(results);
                                      relistFromCache();
                                  },
                              message.Results);
        }

        #endregion

        private void relistFromCache()
        {
            runFeedbackList.Items.Clear();
            foreach (var error in _cache.Errors)
                addFeedbackItem("Build error", formatBuildResult(error), Color.Red, error);

            foreach (var failed in _cache.Failed)
                addFeedbackItem("Test failed", formatTestResult(failed), Color.Red, failed);

            foreach (var warning in _cache.Warnings)
                addFeedbackItem("Build warning", formatBuildResult(warning), Color.Black, warning);
            
            foreach (var ignored in _cache.Ignored)
                addFeedbackItem("Test ignored", formatTestResult(ignored), Color.Black, ignored);
        }

        private void addFeedbackItem(string type, string message, Color colour, object tag)
        {
            var item = runFeedbackList.Items.Add(type);
            item.SubItems.Add(message);
            item.ForeColor = colour;
            item.Tag = tag;
        }

        private string formatBuildResult(BuildItem item)
        {
            var buildMessage = item.Value;
            return string.Format("{0}, {1}", buildMessage.ErrorMessage, buildMessage.File);
        }

        private string formatTestResult(TestItem item)
        {
            var test = item.Value;
            return string.Format("{0} -> {1}", test.Status, test.Name);
        }

        private void runFeedbackList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (runFeedbackList.SelectedItems.Count != 1)
            {
                richTextBoxInfo.Text = "";
                return;
            }

            richTextBoxInfo.Text = runFeedbackList.Items[runFeedbackList.SelectedItems[0].Index]
                .Tag.ToString();
        }
    }
}
