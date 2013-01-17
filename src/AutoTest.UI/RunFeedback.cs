using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using AutoTest.Messages;
using System.IO;
using AutoTest.UI.TextFormatters;

namespace AutoTest.UI
{
    public partial class RunFeedback : UserControl
    {
        private readonly SynchronizationContext _syncContext;
        private TestDetailsForm _testDetails;
        private readonly object _messagLock = new object();
        private bool _isRunning;
        private bool _progressUpdatedExternally;
        private ImageStates _lastInternalState = ImageStates.None;
        private readonly string _progressPicture;

        private bool _showErrors = true;
        private bool _showWarnings = true;
        private bool _showFailing = true;
        private bool _showIgnored = true;

        public event EventHandler<GoToReferenceArgs> GoToReference;
        public event EventHandler<GoToTypeArgs> GoToType;
        public event EventHandler<DebugTestArgs> DebugTest;
        public event EventHandler CancelRun;

        public bool CanGoToTypes { get; set; }
        public bool CanDebug { get; set; }
        public int ListViewWidthOffset { get; set; }
        public bool ShowRunInformation { get; set; }

        public bool ShowIcon
        {
            get
            {
                return pictureMoose.Visible;
            }
            set
            {
                pictureMoose.Visible = value;
                label1.Left = pictureMoose.Left + pictureMoose.Width + 5;

            }
        }

        public RunFeedback()
        {
            _syncContext = AsyncOperationManager.SynchronizationContext;
            InitializeComponent();
            ShowIcon = true;
            CanDebug = false;
            CanGoToTypes = false;
            ShowRunInformation = true;
            if (ListViewWidthOffset > 0)
                listViewFeedback.Width = Width - ListViewWidthOffset;
            organizeListItemBehaviors(listViewFeedback.SelectedItems);
            _progressPicture = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "progress.gif");
        }

        public new void Resize()
        {
            _syncContext.Post(message =>
            {
                organizeListItemBehaviors(listViewFeedback.SelectedItems);
            }, null);
        }

        public void PrepareForFocus()
        {
            _syncContext.Post(x =>
            {
                if (listViewFeedback.Items.Count > 0 && listViewFeedback.SelectedItems.Count == 0)
                    listViewFeedback.Items[0].Selected = true;
                listViewFeedback.Select();
                organizeListItemBehaviors(listViewFeedback.SelectedItems);
            }, null);
        }

        public void ClearList()
        {
            _syncContext.Post(x =>
            {
                listViewFeedback.Items.Clear();
            }, null);
        }

        public void ClearBuilds()
        {
            _syncContext.Post(x =>
            {
                foreach (ListViewItem listItem in listViewFeedback.Items)
                {
                    if (listItem.Tag.GetType() == typeof(CacheBuildMessage))
                    {
                        listViewFeedback.Items.Remove(listItem);
                    }
                }
            }, null);
        }

        public void ClearBuilds(string proj)
        {
            _syncContext.Post(x =>
            {
                var project = x.ToString();
                foreach (ListViewItem listItem in listViewFeedback.Items)
                {
                    if (listItem.Tag.GetType() == typeof(CacheBuildMessage))
                    {
                        var item = (CacheBuildMessage)listItem.Tag;
                        if (item.Project.Equals(project))
                            listViewFeedback.Items.Remove(listItem);
                    }
                }
            }, proj);
        }

        public bool IsInFocus()
        {
            if (Focused)
                return true;
            return Controls.Cast<Control>().Any(control => control.Focused);
        }

        public void SetVisibilityConfiguration(bool showErrors, bool showWarnings, bool showFailingTests, bool showIgnoredTests)
        {
            _showErrors = showErrors;
            _showWarnings = showWarnings;
            _showFailing = showFailingTests;
            _showIgnored = showIgnoredTests;
            _syncContext.Post(x =>
            {
                ClearList();
            }, null);
        }

        public bool isTheSameTestAs(CacheTestMessage original, CacheTestMessage item)
        {
            return original.Assembly.Equals(item.Assembly) && original.Test.Runner.Equals(item.Test.Runner) && original.Test.Name.Equals(item.Test.Name)  && original.Test.DisplayName.Equals(item.Test.DisplayName);
        }

        public void ConsumeMessage(object msg)
        {
            _syncContext.Post(message =>
            {
                lock (_messagLock)
                {
                    try
                    {
                        if (message.GetType() == typeof(CacheMessages))
                            handle((CacheMessages)message);
                        else if (message.GetType() == typeof(LiveTestStatusMessage))
                            handle((LiveTestStatusMessage)message);
                        else if (message.GetType() == typeof(RunStartedMessage))
                            runStarted("Detected file changes...");
                        else if (message.GetType() == typeof(RunFinishedMessage))
                            runFinished((RunFinishedMessage)message);
                        else if (message.GetType() == typeof(RunInformationMessage))
                            runInformationMessage((RunInformationMessage)message);
			            else if (message.GetType() == typeof(BuildRunMessage)) {
				            if (((BuildRunMessage)message).Results.Errors.Length == 0)
					            ClearBuilds(((BuildRunMessage)message).Results.Project); // Make sure no errors remain in log
                        }
                    }
                    catch
                    {
                    }

                }
            }, msg);
        }

        private void handle(CacheMessages cacheMessages)
        {
            Handle(cacheMessages);
            label1.Refresh();
        }

        private void handle(LiveTestStatusMessage liveTestStatusMessage)
        {
            Handle(liveTestStatusMessage);
            label1.Refresh();
        }

        private void runStarted(string x)
        {
            if (!ShowRunInformation)
                x = "processing changes...";
            printMessage(new RunMessages(RunMessageType.Normal, x.ToString()));
            generateSummary(null);
            organizeListItemBehaviors(null);
            clearRunnerTypeAnyItems();
            setProgress(ImageStates.Progress, "processing changes...", false, null, true);
            _isRunning = true;
            organizeListItemBehaviors(listViewFeedback.SelectedItems);
        }

        public void SetProgress(bool on, string information, string picture)
        {
            _progressUpdatedExternally = on;
            var state = _lastInternalState;
            if (on)
                state = ImageStates.Progress;
            setProgress(state, information, true, picture);
        }

        public void SetProgress(bool on, string information, ImageStates imageState)
        {
            _progressUpdatedExternally = on;
            setProgress(imageState, information, true, null);
        }

        private void setProgress(ImageStates state, string information, bool external, string picture)
        {
            setProgress(state, information, external, picture, false);
        }

        private void setProgress(ImageStates state, string information, bool external, string picture, bool force)
        {
            if (!force && _progressUpdatedExternally && !external)
                return;
            if (picture == null)
                picture = _progressPicture;
            if (pictureBoxWorking.ImageLocation != picture)
            {
                pictureBoxWorking.ImageLocation = picture;
                pictureBoxWorking.Refresh();
            }
            pictureBoxRed.Visible = state == ImageStates.Red;
            pictureMoose.Visible = state == ImageStates.Green;
            pictureBoxGray.Visible = state == ImageStates.None;
            pictureBoxWorking.Visible = state == ImageStates.Progress;
            _toolTipProvider.SetToolTip(pictureBoxWorking, information);
            if (!external)
                _lastInternalState = state;
        }

        private void runFinished(RunFinishedMessage x)
        {
            if (((RunFinishedMessage)x).Report.Aborted)
            {
                if (ShowRunInformation)
                {
                    setProgress(ImageStates.None, "", false, null);
                    printMessage(new RunMessages(RunMessageType.Normal, "last build/test run was aborted"));
                }
            }
            else
            {
                var i = getRunFinishedInfo((RunFinishedMessage)x);
                var runType = i.Succeeded ? RunMessageType.Succeeded : RunMessageType.Failed;
                setProgress(runType == RunMessageType.Succeeded ? ImageStates.Green : ImageStates.Red, "", false, null);
                printMessage(new RunMessages(runType, i.Text));
                generateSummary(i.Report);
            }
            _isRunning = false;
            organizeListItemBehaviors(listViewFeedback.SelectedItems);
        }

        private RunFinishedInfo getRunFinishedInfo(RunFinishedMessage message)
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
            var succeeded = !(report.NumberOfBuildsFailed > 0 || report.NumberOfTestsFailed > 0);
            return new RunFinishedInfo(text, succeeded, report);
        }

        private void runInformationMessage(RunInformationMessage x)
        {
            if (!_isRunning)
                return;
            var text = "";
            var message = (RunInformationMessage)x;
            switch (message.Type)
            {
                case InformationType.Build:
                    if (ShowRunInformation)
                        text = string.Format("building {0}", Path.GetFileName(message.Project));
                    break;
                case InformationType.TestRun:
                    text = "testing...";
                    break;
                case InformationType.PreProcessing:
                    if (ShowRunInformation)
                        text = "locating affected tests";
                    break;
            }
            if (text != "") {
                setProgress(ImageStates.Progress, text.ToString(), false, null);
                printMessage(new RunMessages(RunMessageType.Normal, text.ToString()));
            }
        }

        public void PrintMessage(RunMessages message)
        {
            _syncContext.Post(x => printMessage((RunMessages)x), message);
        }

        private void printMessage(RunMessages message)
        {
            var msg = message;
            label1.Text = msg.Message;
            if (msg.Type == RunMessageType.Normal)
            {
                label1.ForeColor = Color.Black;
                label1.Font = new Font(label1.Font, FontStyle.Regular);
            }
            if (msg.Type == RunMessageType.Succeeded)
            {
                label1.ForeColor = Color.Green;
                label1.Font = new Font(label1.Font, FontStyle.Bold);
            }
            if (msg.Type == RunMessageType.Failed)
            {
                label1.ForeColor = Color.Red;
                label1.Font = new Font(label1.Font, FontStyle.Bold);
            }
            label1.Refresh();
        }

        private new void Handle(CacheMessages cache)
        {
            object selected = null;
            if (listViewFeedback.SelectedItems.Count == 1)
                selected = listViewFeedback.SelectedItems[0].Tag;

            listViewFeedback.SuspendLayout();
            removeItems(cache);
            if (_showErrors)
            {
                foreach (var error in cache.ErrorsToAdd)
                    addFeedbackItem("Build error", formatBuildResult(error), Color.Red, error, selected, 0);
            }

            if (_showFailing)
            {
                var position = getFirstItemPosition(new[] { "Test failed", "Build warning", "Test ignored" });
                foreach (var failed in cache.FailedToAdd)
                    addFeedbackItem("Test failed", formatTestResult(failed), Color.Red, failed, selected, position);
            }

            if (_showWarnings)
            {
                var position = getFirstItemPosition(new[] { "Build warning", "Test ignored" });
                foreach (var warning in cache.WarningsToAdd)
                    addFeedbackItem("Build warning", formatBuildResult(warning), Color.Black, warning, selected, position);
            }

            if (_showIgnored)
            {
                var position = getFirstItemPosition(new[] { "Test ignored" });
                foreach (var ignored in cache.IgnoredToAdd)
                    addFeedbackItem("Test ignored", formatTestResult(ignored), Color.Black, ignored, selected, position);
            }
            listViewFeedback.ResumeLayout();
        }

        private new void Handle(LiveTestStatusMessage liveStatus)
        {
            if (!_isRunning)
                return;

            listViewFeedback.SuspendLayout();
            var ofCount = liveStatus.TotalNumberOfTests > 0 ? string.Format(" of {0}", liveStatus.TotalNumberOfTests) : "";
            var testName = liveStatus.CurrentTest;
            if (testName.Trim().Length > 0)
                testName += " in ";
            printMessage(new RunMessages(RunMessageType.Normal, string.Format("testing {3}{0} ({1}{2} tests completed)", Path.GetFileNameWithoutExtension(liveStatus.CurrentAssembly), liveStatus.TestsCompleted, ofCount, testName)));

            if (_showFailing)
            {
                foreach (var test in liveStatus.FailedButNowPassingTests)
                {
                    var testItem = new CacheTestMessage(test.Assembly, test.Test);
                    foreach (ListViewItem item in listViewFeedback.Items)
                    {
                        if (item.Tag.GetType() != typeof(CacheTestMessage))
                            continue;
                        var itm = (CacheTestMessage)item.Tag;
                        if (isTheSameTestAs(itm, testItem))
                        {
                            item.Remove();
                            break;
                        }
                    }
                }

                object selected = null;
                if (listViewFeedback.SelectedItems.Count == 1)
                    selected = listViewFeedback.SelectedItems[0].Tag;
                foreach (var test in liveStatus.FailedTests)
                {
                    var testItem = new CacheTestMessage(test.Assembly, test.Test);
                    ListViewItem toRemove = null;
                    foreach (ListViewItem item in listViewFeedback.Items)
                    {
                        if (item.Tag.GetType() != typeof(CacheTestMessage))
                            continue;
                        var itm = (CacheTestMessage)item.Tag;
                        if (isTheSameTestAs(itm, testItem))
                        {
                            toRemove = item;
                            break;
                        }
                    }
                    int index = toRemove == null ? 0 : toRemove.Index;
                    if (toRemove != null)
                        toRemove.Remove();
                    addFeedbackItem("Test failed", formatTestResult(testItem), Color.Red, testItem, selected, index);
                }
            }
            listViewFeedback.ResumeLayout();
        }

        private void clearRunnerTypeAnyItems()
        {
            var toRemove = new List<ListViewItem>();
            foreach (ListViewItem listItem in listViewFeedback.Items)
            {
                if (listItem.Tag.GetType() == typeof(CacheTestMessage))
                {
                    var item = (CacheTestMessage)listItem.Tag;
                    if (item.Test.Runner == TestRunner.Any)
                        toRemove.Add(listItem);
                }
            }
            toRemove.ForEach(x => listViewFeedback.Items.Remove(x));
        }

        private int getFirstItemPosition(string[] placeBefore)
        {
            int position = 0;
            foreach (ListViewItem listItem in listViewFeedback.Items)
            {
                if ((from p in placeBefore where p.Equals(listItem.Text) select p).Any())
                    return position;
                position++;
            }
            return position;
        }

        private void removeItems(CacheMessages cache)
        {
            var toRemove = new List<ListViewItem>();
            foreach (ListViewItem listItem in listViewFeedback.Items)
            {
                if (listItem.Tag.GetType() == typeof(CacheBuildMessage))
                {
                    var item = (CacheBuildMessage)listItem.Tag;
                    if (cache.ErrorsToRemove.Count(x => x.Equals(item)) > 0 || cache.WarningsToRemove.Count(x => x.Equals(item)) > 0)
                        toRemove.Add(listItem);
                }
                if (listItem.Tag.GetType() == typeof(CacheTestMessage))
                {
                    var item = (CacheTestMessage)listItem.Tag;
                    if (existsIn(cache.TestsToRemove, item))
                        toRemove.Add(listItem);
                }
            }
            toRemove.ForEach(x => listViewFeedback.Items.Remove(x));
        }

        private bool existsIn(IEnumerable<CacheTestMessage> cacheTestMessages, CacheTestMessage item)
        {
            var query = from i in cacheTestMessages
                        where i.Assembly.Equals(item.Assembly) && i.Test.Runner.Equals(item.Test.Runner) && i.Test.Name.Equals(item.Test.Name)
                        select i;
            return query.Any();
        }

        private void addFeedbackItem(string type, string message, Color colour, object tag, object selected, int position)
        {
            if (testExists(tag))
                return;
            var item = listViewFeedback.Items.Insert(position, type);
            item.SubItems.Add(message);
            item.ForeColor = colour;
            item.Tag = tag;
            if (selected != null && tag.GetType() == selected.GetType() && tag.Equals(selected))
                item.Selected = true;
        }

        private bool testExists(object tag)
        {
            if (tag.GetType() != typeof(CacheTestMessage))
                return false;
            var test = (CacheTestMessage)tag;
            return (from ListViewItem item in listViewFeedback.Items where item.Tag.GetType() == typeof (CacheTestMessage) select (CacheTestMessage) item.Tag).Any(itm => isTheSameTestAs(test, itm));
        }

        private string formatBuildResult(CacheBuildMessage item)
        {
            return string.Format("{0}, {1}", item.BuildItem.ErrorMessage, item.BuildItem.File);
        }

        private string formatTestResult(CacheTestMessage item)
        {
            return string.Format("{0} -> ({1}) {2}", item.Test.Status, item.Test.Runner.ToString(), item.Test.DisplayName);
        }

        private void listViewFeedback_SelectedIndexChanged(object sender, EventArgs e)
        {
            organizeListItemBehaviors(listViewFeedback.SelectedItems);
        }

        private void organizeListItemBehaviors(ListView.SelectedListViewItemCollection collection)
        {
            using (var handler = new ListItemBehaviourHandler(this))
            {
                handler.Organize(collection, _isRunning);
            }
        }

        private void listViewFeedback_DoubleClick(object sender, EventArgs e)
        {
            if (listViewFeedback.SelectedItems.Count != 1)
                return;

            var item = listViewFeedback.SelectedItems[0].Tag;
            if (item.GetType() == typeof(CacheBuildMessage))
                goToBuildItemReference((CacheBuildMessage)item);
            if (item.GetType() == typeof(CacheTestMessage))
                goToTestItemReference((CacheTestMessage)item);
        }

        private void goToBuildItemReference(CacheBuildMessage buildItem)
        {
            goToReference(buildItem.BuildItem.File, buildItem.BuildItem.LineNumber, buildItem.BuildItem.LinePosition);
        }

        private void goToTestItemReference(CacheTestMessage testItem)
        {
            if (testItem.Test.StackTrace.Length > 0)
            {
				var exactLine = getMatchingStackLine(testItem);
				if (exactLine != null) {
					goToReference(exactLine.File, exactLine.LineNumber, 0);
					return;
				}
				
                if (CanGoToTypes)
                    if (goToType(testItem.Assembly, testItem.Test.Name))
                        return;
            }
        }

		private IStackLine getMatchingStackLine(CacheTestMessage testItem)
		{
			foreach (var line in testItem.Test.StackTrace) {
				if (line.Method.Equals(testItem.Test.Name))
					return line;
			}
            var lastWithLine = testItem.Test.StackTrace.LastOrDefault(x => x.LineNumber > 0);
            if (lastWithLine != null)
                return lastWithLine;

			return null;
		}

        private void goToReference(string file, int lineNumber, int column)
        {
            if (GoToReference != null)
                GoToReference(this, new GoToReferenceArgs(new CodePosition(file, lineNumber, column)));
        }

        private bool goToType(string assembly, string typename)
        {
            var type = typename.Replace("+", ".");
            var args = new GoToTypeArgs(assembly, type);
            if (GoToType != null)
                GoToType(this, args);
            return args.Handled;
        }

        private void generateSummary(RunReport report)
        {
            if (report == null)
            {
                _toolTipProvider.RemoveAll();
                return;
            }

            var builder = new SummaryBuilder(report);
            _toolTipProvider.SetToolTip(label1, builder.Build());
        }

        private void linkLabelDebugTest_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DebugTest == null)
                return;

            if (listViewFeedback.SelectedItems.Count != 1)
                return;
            if (listViewFeedback.SelectedItems[0].Tag.GetType() != typeof(CacheTestMessage))
                return;
            
            var test = (CacheTestMessage)listViewFeedback.SelectedItems[0].Tag;
            DebugTest(this, new DebugTestArgs(test));
        }

        private void listViewFeedback_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode.Equals(Keys.Enter))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    listViewFeedback_DoubleClick(this, new EventArgs());
                    return;
                }
                if (e.KeyCode.Equals(Keys.D))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (linkLabelDebugTest.Visible == true)
                        linkLabelDebugTest_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link(0, 1)));
                    return;
                }

                if (e.KeyCode.Equals(Keys.I))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (linkLabelTestDetails.Visible)
                        linkLabelTestDetails_LinkClicked(this, new LinkLabelLinkClickedEventArgs(null));
                    else if (linkLabelErrorDescription.Visible)
                        linkLabelErrorDescription_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link(0, 1)));
                    return;
                }

                if (e.KeyCode.Equals(Keys.A))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (linkLabelCancelRun.Visible)
                        linkLabelSystemMessages_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link(0, 1)));
                    return;
                }
                if (e.KeyCode.Equals(Keys.K) || (e.Shift && e.KeyCode.Equals(Keys.F8)))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    SendKeys.Send("{UP}");
                    if (e.KeyCode.Equals(Keys.F8))
                        listViewFeedback_DoubleClick(this, new EventArgs());
                    return;
                }
                if (e.KeyCode.Equals(Keys.J) || (e.KeyCode.Equals(Keys.F8)))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    SendKeys.Send("{DOWN}");
                    if (e.KeyCode.Equals(Keys.F8))
                        listViewFeedback_DoubleClick(this, new EventArgs());
                }
            }
            catch
            {
                // Pft who logs!?
            }
        }

        private void linkLabelTestDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var item = (CacheTestMessage)listViewFeedback.SelectedItems[0].Tag;
                var builder = new DetailBuilder(item);
                var details = builder.Text;
                var links = builder.Links;
                if (CanGoToTypes)
                    links.Insert(0, new Link(details.IndexOf(item.Test.Name), details.IndexOf(item.Test.Name) + item.Test.Name.Length, item.Assembly, item.Test.Name));
                showDetailsWindow(details, "Test output", links, Screen.PrimaryScreen.WorkingArea.Width > 1024 ? Screen.PrimaryScreen.WorkingArea.Width - 500 : Screen.PrimaryScreen.WorkingArea.Width);
            }
            catch
            {
                // Pft who logs!?
            }
        }

        private void linkLabelErrorDescription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var item = (CacheBuildMessage)listViewFeedback.SelectedItems[0].Tag;
                var builder = new DetailBuilder(item);
                var details = builder.Text;
                var links = builder.Links;
                showDetailsWindow(details, "Build output", links, Screen.PrimaryScreen.WorkingArea.Width > 1024 ? 1024 : Screen.PrimaryScreen.WorkingArea.Width);
            }
            catch
            {
                // Pft who logs!?
            }
        }

        private void showDetailsWindow(string message, string caption, List<Link> links, int maxWidth)
        {
            var hasTestDetails = _testDetails != null;
            var x = -1;
            var y = -1;
            if (hasTestDetails)
            {
                x = _testDetails.Left;
                y = _testDetails.Top;
            }

            _testDetails = new TestDetailsForm((file, line) => goToReference(file, line, 0), goToType, x, y, message, caption, links, maxWidth);
            _testDetails.Show();
            _testDetails.BringToFront();
        }

        private void linkLabelSystemMessages_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CancelRun != null)
                CancelRun(this, new EventArgs());
        }

        private void RunFeedback_Resize(object sender, EventArgs e)
        {
            organizeListItemBehaviors(listViewFeedback.SelectedItems);
        }

        private void listViewFeedback_Resize(object sender, EventArgs e)
        {
            listViewFeedback.Columns[1].Width = listViewFeedback.Width - (listViewFeedback.Columns[0].Width + 25);
        }
    }

    public class GoToReferenceArgs : EventArgs
    {
        public CodePosition Position { get; private set; }
        public GoToReferenceArgs(CodePosition position) { Position = position; }
    }

    public class GoToTypeArgs : EventArgs
    {
        public bool Handled = true;
        public string Assembly { get; private set; }
        public string TypeName { get; private set; }
        public GoToTypeArgs(string assembly, string typename) { Assembly = assembly; TypeName = typename; }
    }

    public class DebugTestArgs : EventArgs
    {
        public CacheTestMessage Test { get; private set; }
        public DebugTestArgs(CacheTestMessage test) { Test = test; }
    }
}
