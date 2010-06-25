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
        private IInformationForm _informationForm;
        private Cache _cache = new Cache();

        private int _rightSpacing = 0;
        private int _listBottomSpacing = 0;
        private int _infoBottomSpacing = 0;

        public FeedbackForm(IDirectoryWatcher watcher, IConfiguration configuration, IRunFeedbackPresenter runPresenter, IInformationForm informationForm)
        {
            _syncContext = AsyncOperationManager.SynchronizationContext;
            _watcher = watcher;
            _runPresenter = runPresenter;
            _runPresenter.View = this;
            _informationForm = informationForm;
            _informationForm.MessageArrived += new EventHandler<MessageRecievedEventArgs>(_informationForm_MessageArrived);
            InitializeComponent();
            _watcher.Watch(configuration.DirectoryToWatch);
            readFormSpacing();
        }

        private void readFormSpacing()
        {
            _rightSpacing = Width - (runFeedbackList.Left + runFeedbackList.Width);
            _listBottomSpacing = linkLabelInfo.Top - (runFeedbackList.Top + runFeedbackList.Height);
            _infoBottomSpacing = Height - (linkLabelInfo.Top + linkLabelInfo.Height);
        }

        void _informationForm_MessageArrived(object sender, MessageRecievedEventArgs e)
        {
            buttonInformation.Text = "|||";
            if (e.Type.Equals(MessageType.Warning))
                buttonInformation.ForeColor = Color.Yellow;
            else if (e.Type.Equals(MessageType.Error))
                buttonInformation.ForeColor = Color.Red;
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
            setInfoText("");
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
                setInfoText("");
                return;
            }

            setInfoText(runFeedbackList.Items[runFeedbackList.SelectedItems[0].Index].Tag.ToString());
        }

        private void setInfoText(string text)
        {
            int previousHeight = linkLabelInfo.Height;
            var parser = new LinkParser(text);
            var links = parser.Parse();
            linkLabelInfo.Text = parser.ParsedText;
            linkLabelInfo.LinkArea = new LinkArea(0, 0);
            foreach (var link in links)
                linkLabelInfo.Links.Add(link.Start, link.Length);
            var difference = linkLabelInfo.Height - previousHeight;
            Height = Height + difference;
        }

        private void FeedbackForm_Resize(object sender, EventArgs e)
        {
            linkLabelInfo.MaximumSize = new Size(Width - (linkLabelInfo.Left + _rightSpacing), 0);
            linkLabelInfo.Top = Height - (linkLabelInfo.Height + _infoBottomSpacing);
            runFeedbackList.Height = linkLabelInfo.Top - (runFeedbackList.Top + _listBottomSpacing);
            runFeedbackList.Width = Width - (runFeedbackList.Left + _rightSpacing);
        }

        private void linkLabelInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (runFeedbackList.SelectedItems.Count != 1)
                return;
            var item = (IItem) runFeedbackList.SelectedItems[0].Tag;
            item.HandleLink(linkLabelInfo.Text.Substring(e.Link.Start, e.Link.Length));
        }

        private void buttonInformation_Click(object sender, EventArgs e)
        {
            buttonInformation.Text = "";
            buttonInformation.ForeColor = BackColor;
            var form = _informationForm.Form;
            if (form.Visible == false)
                form.Visible = true;
            else
                form.Show(this);
        }

        private void FeedbackForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _informationForm.Form.Dispose();
        }
    }
}
