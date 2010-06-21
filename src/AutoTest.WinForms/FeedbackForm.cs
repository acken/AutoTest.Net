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

namespace AutoTest.WinForms
{
    public partial class FeedbackForm : Form, IOverviewForm, IRunFeedbackView
    {
        private SynchronizationContext _syncContext;
        private IRunFeedbackPresenter _runPresenter;
        private IDirectoryWatcher _watcher;
        private IConfiguration _configuration;

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
            _syncContext.Post(s => labelRunState.Text = "Detected changes, running..", null);
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
                },
                message.Report);
        }

        public void  RecievingBuildMessage(BuildRunMessage message)
        {
            _syncContext.Post(m =>
                                  {
                                      var results = (BuildRunResults) m;
                                      foreach (var error in results.Errors)
                                      {
                                          runFeedbackList.Items.Add("Error")
                                              .SubItems.Add(error.ErrorMessage);
                                      }
                                      foreach (var warning in results.Warnings)
                                      {
                                          runFeedbackList.Items.Add("Warning")
                                              .SubItems.Add(warning.ErrorMessage);
                                      }
                                  },
                              message.Results);
        }

        public void  RecievingTestRunMessage(TestRunMessage message)
        {
            _syncContext.Post(m =>
                                  {
                                      var results = (TestRunResults) m;
                                      foreach (var failed in results.Failed)
                                      {
                                          runFeedbackList.Items.Add("Failed")
                                              .SubItems.Add(failed.Name);
                                      }
                                      foreach (var ignored in results.Ignored)
                                      {
                                          runFeedbackList.Items.Add("Ignored")
                                              .SubItems.Add(ignored.Name);
                                      }
                                  },
                              message.Results);
        }

        #endregion
    }
}
