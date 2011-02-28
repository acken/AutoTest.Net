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
using System.IO;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Core.Notifiers;
using AutoTest.Messages;

namespace AutoTest.WinForms
{
    public partial class FeedbackForm : Form, IOverviewForm, IRunFeedbackView
    {
		private object _padLock = new object();
		private const int GDI_SIZE_LIMIT = 3200;

        private string _directoryToWatch;
        private SynchronizationContext _syncContext;
        private IRunFeedbackPresenter _runPresenter;
        private IDirectoryWatcher _watcher;
		private IConfiguration _configuration;
        private IInformationForm _informationForm;
        private IRunResultCache _runResultCache;
		private IMessageBus _bus;
		private ISendNotifications _notifier;

        private ToolTip _toolTipProvider = new ToolTip();
        private bool _isRefreshingFeedback = false;

        private int _rightSpacing = 0;
        private int _listBottomSpacing = 0;
        private int _infoBottomSpacing = 0;
		private bool _unlockedRunning = false;
		private bool _running { get { lock(_padLock) { return _unlockedRunning; } } set { lock(_padLock) { _unlockedRunning = value; } } }

        public FeedbackForm(IDirectoryWatcher watcher, IConfiguration configuration, IRunFeedbackPresenter runPresenter, IInformationForm informationForm, IRunResultCache runResultCache, IMessageBus bus, ISendNotifications notifier)
        {
            _syncContext = AsyncOperationManager.SynchronizationContext;
            _toolTipProvider.AutoPopDelay = 30000;
            _watcher = watcher;
			_configuration = configuration;
            _runResultCache = runResultCache;
			_bus = bus;
			_notifier = notifier;
            _runPresenter = runPresenter;
            _runPresenter.View = this;
            _informationForm = informationForm;
            _informationForm.MessageArrived += new EventHandler<MessageRecievedEventArgs>(_informationForm_MessageArrived);
            InitializeComponent();
            readFormSpacing();
			FeedbackForm_Resize(this, new EventArgs());
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
            if (e.Type.Equals(MessageType.Warning) && buttonInformation.ForeColor != Color.Red)
                buttonInformation.ForeColor = Color.Yellow;
            else if (e.Type.Equals(MessageType.Error))
                buttonInformation.ForeColor = Color.Red;
        }

        #region IOverviewForm Members

        public void SetWatchDirectory(string directory)
        {
            _directoryToWatch = directory;
            _watcher.Watch(_directoryToWatch);
            _configuration.ValidateSettings();
        }

        public Form Form
        {
            get { return this; }
        }

        #endregion
    
        #region IRunFeedbackView Members

        public void RecievingFileChangeMessage(FileChangeMessage message)
        {
        }

        public void  RecievingRunStartedMessage(RunStartedMessage message)
        {
            _syncContext.Post(s =>
                                  {
                                      setRunInProgressFeedback("");
									  _running = true;
                                      generateSummary(null);
									  if (_configuration.NotifyOnRunStarted)
									  	runNotification("Detected changes, running..", null);
                                  }, null);
        }

        private void setRunInProgressFeedback(string additionalInfo)
        {
			if (!_running)
				return;
            var text = "Detected changes, running..";
            if (additionalInfo.Length > 0)
                text += string.Format(" ({0})", additionalInfo);
            labelRunState.Text = text;
            labelRunState.ForeColor = Color.Black;
        }
		 
        public void  RecievingRunFinishedMessage(RunFinishedMessage message)
        {
            _syncContext.Post(
                x =>
                {
                    var report = (RunReport) x;
					var msg = string.Format(
                        "Ran {0} build(s) ({1} succeeded, {2} failed) and {3} test(s) ({4} passed, {5} failed, {6} ignored)",
                        report.NumberOfProjectsBuilt,
                        report.NumberOfBuildsSucceeded,
                        report.NumberOfBuildsFailed,
                        report.NumberOfTestsRan,
                        report.NumberOfTestsPassed,
                        report.NumberOfTestsFailed,
                        report.NumberOfTestsIgnored);
					_running = false;
                    labelRunState.Text = msg;
					if (report.NumberOfBuildsFailed > 0 || report.NumberOfTestsFailed > 0)
						labelRunState.ForeColor = Color.Red;
					if (report.NumberOfBuildsFailed == 0 && report.NumberOfTestsFailed == 0 && report.NumberOfTestsIgnored > 0)
						labelRunState.ForeColor = Color.YellowGreen;
					if (report.NumberOfBuildsFailed == 0 && report.NumberOfTestsFailed == 0 && report.NumberOfTestsIgnored == 0)
						labelRunState.ForeColor = Color.Green;
                    generateSummary(report);
					if (_configuration.NotifyOnRunCompleted)
						runNotification(msg, report);
                },
                message.Report);
        }

        private void generateSummary(RunReport report)
        {
            if (report == null)
            {
                _toolTipProvider.RemoveAll();
                return;
            }

            var builder = new SummaryBuilder(report);
            _toolTipProvider.SetToolTip(labelRunState, builder.Build());
        }

        public void  RecievingBuildMessage(BuildRunMessage message)
        {
            _syncContext.Post(m => relistFromCache(), null);
        }

        public void  RecievingTestRunMessage(TestRunMessage message)
        {
            _syncContext.Post(m => relistFromCache(), null);
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
            _syncContext.Post(m => setRunInProgressFeedback(m.ToString()), text);
        }

        #endregion

        private void relistFromCache()
        {
            _isRefreshingFeedback = true;

            IItem selected = null;
            if (runFeedbackList.SelectedItems.Count == 1)
                selected = (IItem) runFeedbackList.SelectedItems[0].Tag;

            runFeedbackList.Items.Clear();
            foreach (var error in _runResultCache.Errors)
                addFeedbackItem("Build error", formatBuildResult(error), Color.Red, error, selected);

            foreach (var failed in _runResultCache.Failed)
                addFeedbackItem("Test failed", formatTestResult(failed), Color.Red, failed, selected);

            foreach (var warning in _runResultCache.Warnings)
                addFeedbackItem("Build warning", formatBuildResult(warning), Color.Black, warning, selected);
            
            foreach (var ignored in _runResultCache.Ignored)
                addFeedbackItem("Test ignored", formatTestResult(ignored), Color.Black, ignored, selected);

            if (runFeedbackList.Items.Count != 1)
                setInfoText("");

            _isRefreshingFeedback = false;

            setInfoFromSelectedItem();
        }

        private void addFeedbackItem(string type, string message, Color colour, IItem tag, IItem selected)
        {
            var item = runFeedbackList.Items.Add(type);
            item.SubItems.Add(message);
            item.ForeColor = colour;
            item.Tag = tag;
            if (selected != null && tag.GetType().Equals(selected.GetType()) && tag.Equals(selected))
                item.Selected = true;
        }

        private string formatBuildResult(BuildItem item)
        {
            var buildMessage = item.Value;
            return string.Format("{0}, {1}", buildMessage.ErrorMessage, buildMessage.File);
        }

        private string formatTestResult(TestItem item)
        {
            var test = item.Value;
            return string.Format("{0} -> ({2}) {1}", test.Status, test.Name, test.Runner);
        }

        private void runFeedbackList_SelectedIndexChanged(object sender, EventArgs e)
        {
            setInfoFromSelectedItem();
        }

        private void setInfoFromSelectedItem()
        {
            if (_isRefreshingFeedback)
                return;

            if (runFeedbackList.SelectedItems.Count != 1)
            {
                setInfoText("");
                return;
            }

            setInfoText(runFeedbackList.Items[runFeedbackList.SelectedItems[0].Index].Tag.ToString());
        }
		
        private void setInfoText(string text)
        {
			try
			{
	            int previousHeight = linkLabelInfo.Height;
	            var parser = new LinkParser(text);
	            var links = parser.Parse();
				if (parser.ParsedText.Length > GDI_SIZE_LIMIT)
					linkLabelInfo.Text = parser.ParsedText.Substring(0, GDI_SIZE_LIMIT);
				else
	            	linkLabelInfo.Text = parser.ParsedText;
	            linkLabelInfo.LinkArea = new LinkArea(0, 0);
	            foreach (var link in links)
				{
					if (link.Start + link.Length <= GDI_SIZE_LIMIT)
						linkLabelInfo.Links.Add(link.Start, link.Length);
				}
	            var difference = linkLabelInfo.Height - previousHeight;
	            Height = Height + difference;
			}
			catch (Exception exception)
			{
				_bus.Publish(new ErrorMessage(exception));
			}
        }

        private void FeedbackForm_Resize(object sender, EventArgs e)
        {
			labelRunState.Width = Width - ((Width - buttonInformation.Left) + _rightSpacing);
            linkLabelInfo.MaximumSize = new Size(Width - (linkLabelInfo.Left + _rightSpacing), 0);
			// This is truely horrendous but it does the job for now
			if (Environment.OSVersion.Platform.Equals(PlatformID.Unix))
			{
				linkLabelInfo.Top = Height - (linkLabelInfo.Height + _infoBottomSpacing + 60);
				runFeedbackList.Height = linkLabelInfo.Top - (runFeedbackList.Top + _listBottomSpacing);
            	runFeedbackList.Width = Width - (runFeedbackList.Left + _rightSpacing + 10);
			}
			else
			{
				linkLabelInfo.Top = Height - (linkLabelInfo.Height + _infoBottomSpacing);
            	runFeedbackList.Height = linkLabelInfo.Top - (runFeedbackList.Top + _listBottomSpacing);
            	runFeedbackList.Width = Width - (runFeedbackList.Left + _rightSpacing);
			}
        }

        private void linkLabelInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (runFeedbackList.SelectedItems.Count != 1)
                return;
            var item = (IItem) runFeedbackList.SelectedItems[0].Tag;
            item.HandleLink(linkLabelInfo.Text.Substring(e.Link.Start, e.Link.Length));
			// If we do not set focus away from this linklabel on click
			// mono will cause the app to die the next time we click the
			// listview after a new run.
			runFeedbackList.Focus();
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
		
		private void runNotification(string msg, RunReport report) {
			var notifyType = getNotify(report);			
			_notifier.Notify(msg, notifyType);			
		}
		
		private NotificationType getNotify(RunReport report)
		{
			if (report == null)
				return NotificationType.Information;
			if (report.NumberOfBuildsFailed > 0 || report.NumberOfTestsFailed > 0)
				return NotificationType.Red;
			if (report.NumberOfTestsIgnored > 0)
				return NotificationType.Yellow;
			return NotificationType.Green;
		}
    }
}
