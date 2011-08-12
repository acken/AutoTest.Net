using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.UI
{
    public partial class RunFeedback : UserControl
    {
        private readonly SynchronizationContext _syncContext;
        private TestDetailsForm _testDetails;
        private readonly object _messagLock = new object();
        private bool _isRunning;

        private bool _showErrors = true;
        private bool _showWarnings = true;
        private bool _showFailing = true;
        private bool _showIgnored = true;

        public event EventHandler<GoToReferenceArgs> GoToReference;
        public event EventHandler<GoToTypeArgs> GoToType;
        public event EventHandler<DebugTestArgs> DebugTest;
        public event EventHandler ShowSystemWindow;

        public bool CanGoToTypes { get; set; }
        public bool CanDebug { get; set; }
        public int ListViewWidthOffset { get; set; }

        public bool ShowIcon
        {
            get
            {
                return pictureMoose.Visible;
            }
            set
            {
                pictureMoose.Visible = value;
                if (value == false)
                    label1.Left = pictureMoose.Left;
                else
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
            if (ListViewWidthOffset > 0)
                listViewFeedback.Width = Width - ListViewWidthOffset;
            organizeListItemBehaviors(listViewFeedback.SelectedItems);
        }

        public new void Resize()
        {
            organizeListItemBehaviors(listViewFeedback.SelectedItems);
        }

        public void PrepareForFocus()
        {
            if (listViewFeedback.Items.Count > 0 && listViewFeedback.SelectedItems.Count == 0)
                listViewFeedback.Items[0].Selected = true;
            listViewFeedback.Select();
            organizeListItemBehaviors(listViewFeedback.SelectedItems);
        }

        public void ClearList()
        {
            listViewFeedback.Items.Clear();
        }

        public void ClearBuilds()
        {
            foreach (ListViewItem listItem in listViewFeedback.Items)
            {
                if (listItem.Tag.GetType().Equals(typeof(CacheBuildMessage)))
                {
                    listViewFeedback.Items.Remove(listItem);
                }
            }
        }

        public void ClearBuilds(string project)
        {
            foreach (ListViewItem listItem in listViewFeedback.Items)
            {
                if (listItem.Tag.GetType().Equals(typeof(CacheBuildMessage)))
                {
                    var item = (CacheBuildMessage)listItem.Tag;
                    if (item.Project.Equals(project))
                        listViewFeedback.Items.Remove(listItem);
                }
            }
        }

        public bool IsInFocus()
        {
            if (Focused)
                return true;
            foreach (Control control in Controls)
            {
                if (control.Focused)
                    return true;
            }
            return false;
        }

        public void SetVisibilityConfiguration(bool showErrors, bool showWarnings, bool showFailingTests, bool showIgnoredTests)
        {
            _showErrors = showErrors;
            _showWarnings = showWarnings;
            _showFailing = showFailingTests;
            _showIgnored = showIgnoredTests;
            ClearList();
        }

        public bool isTheSameTestAs(CacheTestMessage original, CacheTestMessage item)
        {
            return original.Assembly.Equals(item.Assembly) && original.Test.Runner.Equals(item.Test.Runner) && original.Test.Name.Equals(item.Test.Name);
        }

        public void ConsumeMessage(object message)
        {
            if (message.GetType().Equals(typeof(CacheMessages)))
                handle((CacheMessages)message);
            else if (message.GetType().Equals(typeof(LiveTestStatusMessage)))
                handle((LiveTestStatusMessage)message);
            else if (message.GetType().Equals(typeof(RunStartedMessage)))
                runStarted("Detected file changes...");
            else if (message.GetType().Equals(typeof(RunFinishedMessage)))
                runFinished((RunFinishedMessage)message);
            else if (message.GetType().Equals(typeof(RunInformationMessage)))
                runInformationMessage((RunInformationMessage)message);
        }

        private void handle(CacheMessages cacheMessages)
        {
            _syncContext.Post(x =>
            {
                lock (_messagLock)
                {
                    Handle((CacheMessages)x);
                    label1.Refresh();
                }
            }, cacheMessages);
        }

        private void handle(LiveTestStatusMessage liveTestStatusMessage)
        {
            _syncContext.Post(x =>
            {
                lock (_messagLock)
                {
                    Handle((LiveTestStatusMessage)x);
                    label1.Refresh();
                }
            }, liveTestStatusMessage);
        }

        private void runStarted(string text)
        {
            _syncContext.Post(x =>
            {
                printMessage(new RunMessages(RunMessageType.Normal, x.ToString()));
                generateSummary(null);
                organizeListItemBehaviors(null);
                _isRunning = true;
            }, text);
        }

        private void runFinished(RunFinishedMessage msg)
        {
            _syncContext.Post(x =>
            {
                var i = getRunFinishedInfo((RunFinishedMessage)x);
                var runType = i.Succeeded ? RunMessageType.Succeeded : RunMessageType.Failed;
                printMessage(new RunMessages(runType, i.Text));
                generateSummary(i.Report);
                _isRunning = false;
            }, msg);
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
            var succeeded = report.NumberOfBuildsFailed > 0 || report.NumberOfTestsFailed > 0 ? false : true;
            return new RunFinishedInfo(text, succeeded, report);
        }

        private void runInformationMessage(RunInformationMessage msg)
        {
            _syncContext.Post(x =>
            {
                if (!_isRunning)
                    return;
                var text = "";
                var message = (RunInformationMessage)x;
                switch (message.Type)
                {
                    case InformationType.Build:
                        text = string.Format("building {0}", Path.GetFileName(message.Project));
                        break;
                    case InformationType.TestRun:
                        text = "testing...";
                        break;
                    case InformationType.PreProcessing:
                        text = "locating affected tests";
                        break;
                }
                printMessage(new RunMessages(RunMessageType.Normal, text.ToString()));
            }, msg);
        }

        public void PrintMessage(RunMessages message)
        {
            _syncContext.Post(x =>
            {
                printMessage((RunMessages)x);
            }, message);
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

            SuspendLayout();
            removeItems(cache);
            if (_showErrors)
            {
                foreach (var error in cache.ErrorsToAdd)
                    addFeedbackItem("Build error", formatBuildResult(error), Color.Red, error, selected, 0);
            }

            if (_showFailing)
            {
                var position = getFirstItemPosition(new string[] { "Test failed", "Build warning", "Test ignored" });
                foreach (var failed in cache.FailedToAdd)
                    addFeedbackItem("Test failed", formatTestResult(failed), Color.Red, failed, selected, position);
            }

            if (_showWarnings)
            {
                var position = getFirstItemPosition(new string[] { "Build warning", "Test ignored" });
                foreach (var warning in cache.WarningsToAdd)
                    addFeedbackItem("Build warning", formatBuildResult(warning), Color.Black, warning, selected, position);
            }

            if (_showIgnored)
            {
                var position = getFirstItemPosition(new string[] { "Test ignored" });
                foreach (var ignored in cache.IgnoredToAdd)
                    addFeedbackItem("Test ignored", formatTestResult(ignored), Color.Black, ignored, selected, position);
            }
            ResumeLayout();
        }

        private new void Handle(LiveTestStatusMessage liveStatus)
        {
            if (!_isRunning)
                return;
            SuspendLayout();
            var ofCount = liveStatus.TotalNumberOfTests > 0 ? string.Format(" of {0}", liveStatus.TotalNumberOfTests) : "";
            printMessage(new RunMessages(RunMessageType.Normal, string.Format("testing {0} ({1}{2} tests completed)", Path.GetFileNameWithoutExtension(liveStatus.CurrentAssembly), liveStatus.TestsCompleted, ofCount)));

            if (_showFailing)
            {
                foreach (var test in liveStatus.FailedButNowPassingTests)
                {
                    var testItem = new CacheTestMessage(test.Assembly, test.Test);
                    foreach (ListViewItem item in listViewFeedback.Items)
                    {
                        if (!item.Tag.GetType().Equals(typeof(CacheTestMessage)))
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
                        if (!item.Tag.GetType().Equals(typeof(CacheTestMessage)))
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
            ResumeLayout();
        }

        private int getFirstItemPosition(string[] placeBefore)
        {
            int position = 0;
            foreach (ListViewItem listItem in listViewFeedback.Items)
            {
                if ((from p in placeBefore where p.Equals(listItem.Text) select p).Count() > 0)
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
                if (listItem.Tag.GetType().Equals(typeof(CacheBuildMessage)))
                {
                    var item = (CacheBuildMessage)listItem.Tag;
                    if (cache.ErrorsToRemove.Count(x => x.Equals(item)) > 0 || cache.WarningsToRemove.Count(x => x.Equals(item)) > 0)
                        toRemove.Add(listItem);
                }
                if (listItem.Tag.GetType().Equals(typeof(CacheTestMessage)))
                {
                    var item = (CacheTestMessage)listItem.Tag;
                    if (existsIn(cache.TestsToRemove, item))
                        toRemove.Add(listItem);
                }
            }
            toRemove.ForEach(x => listViewFeedback.Items.Remove(x));
        }

        private bool existsIn(CacheTestMessage[] cacheTestMessages, CacheTestMessage item)
        {
            var query = from i in cacheTestMessages
                        where i.Assembly.Equals(item.Assembly) && i.Test.Runner.Equals(item.Test.Runner) && i.Test.Name.Equals(item.Test.Name)
                        select i;
            return query.Count() > 0;
        }

        private void addFeedbackItem(string type, string message, Color colour, object tag, object selected, int position)
        {
            if (testExists(tag))
                return;
            var item = listViewFeedback.Items.Insert(position, type);
            item.SubItems.Add(message);
            item.ForeColor = colour;
            item.Tag = tag;
            if (selected != null && tag.GetType().Equals(selected.GetType()) && tag.Equals(selected))
                item.Selected = true;
        }

        private bool testExists(object tag)
        {
            if (!tag.GetType().Equals(typeof(CacheTestMessage)))
                return false;
            var test = (CacheTestMessage)tag;
            foreach (ListViewItem item in listViewFeedback.Items)
            {
                if (!item.Tag.GetType().Equals(typeof(CacheTestMessage)))
                    continue;
                var itm = (CacheTestMessage)item.Tag;
                if (isTheSameTestAs(test, itm))
                    return true;
            }
            return false;
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
                handler.Organize(collection);
            }
        }

        private void listViewFeedback_DoubleClick(object sender, EventArgs e)
        {
            if (listViewFeedback.SelectedItems.Count != 1)
                return;

            var item = listViewFeedback.SelectedItems[0].Tag;
            if (item.GetType().Equals(typeof(CacheBuildMessage)))
                goToBuildItemReference((CacheBuildMessage)item);
            if (item.GetType().Equals(typeof(CacheTestMessage)))
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
                // TODO: Add this again when type cache is faster
                if (CanGoToTypes)
                    goToType(testItem.Assembly, testItem.Test.Name);
                else
                    goToReference(testItem.Test.StackTrace[0].File, testItem.Test.StackTrace[0].LineNumber, 0);
            }
        }

        private void goToReference(string file, int lineNumber, int column)
        {
            if (GoToReference != null)
                GoToReference(this, new GoToReferenceArgs(new CodePosition(file, lineNumber, column)));
        }

        private void goToType(string assembly, string typename)
        {
            if (GoToType != null)
                GoToType(this, new GoToTypeArgs(assembly, typename));
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
            if (!listViewFeedback.SelectedItems[0].Tag.GetType().Equals(typeof(CacheTestMessage)))
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
                    if (linkLabelTestDetails.Visible == true)
                        linkLabelTestDetails_LinkClicked(this, new LinkLabelLinkClickedEventArgs(null));
                    else if (linkLabelErrorDescription.Visible == true)
                        linkLabelErrorDescription_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link(0, 1)));
                    return;
                }

                if (e.KeyCode.Equals(Keys.S))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (linkLabelSystemMessages.Visible == true)
                        linkLabelSystemMessages_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link(0, 1)));
                    return;
                }
                if (e.KeyCode.Equals(Keys.K))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    if (listViewFeedback.Items.Count == 0)
                        return;
                    if (listViewFeedback.SelectedItems.Count != 1)
                        return;
                    if (listViewFeedback.SelectedItems[0].Index == 0)
                        return;
                    var selectedIndex = listViewFeedback.SelectedItems[0].Index;
                    listViewFeedback.SelectedItems[0].Selected = false;
                    listViewFeedback.Items[selectedIndex - 1].Selected = true;
                    return;
                }
                if (e.KeyCode.Equals(Keys.J))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (listViewFeedback.Items.Count == 0)
                        return;
                    if (listViewFeedback.SelectedItems.Count != 1)
                        return;
                    if (listViewFeedback.SelectedItems[0].Index == (listViewFeedback.Items.Count - 1))
                        return;
                    var selectedIndex = listViewFeedback.SelectedItems[0].Index;
                    listViewFeedback.SelectedItems[0].Selected = false;
                    listViewFeedback.Items[selectedIndex + 1].Selected = true;
                    return;
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
                var details = getTestDetails(item);
                var links = getLinks(ref details);
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
                var details = getBuildDetails(item);
                var links = getLinks(ref details);
                showDetailsWindow(details, "Build output", links, Screen.PrimaryScreen.WorkingArea.Width > 1024 ? 1024 : Screen.PrimaryScreen.WorkingArea.Width);
            }
            catch
            {
                // Pft who logs!?
            }
        }

        private List<Link> getLinks(ref string message)
        {
            message = message.Trim();
            if (message.EndsWith(Environment.NewLine))
                message = message.Substring(0, message.Length - Environment.NewLine.Length);
            var parser = new LinkParser(message);
            var links = parser.Parse();
            message = parser.ParsedText;
            var textForLambda = message;
            return links
                .Where(x => containsLink(textForLambda.Substring(x.Start, x.Length)))
                .Select(x =>
                new Link(
                    getBeginningOfLine(textForLambda, x.Start),
                    getEndOfLine(textForLambda, x.Start + x.Length),
                    getFile(textForLambda.Substring(x.Start, x.Length)),
                    getLine(textForLambda.Substring(x.Start, x.Length)))).ToList();
        }

        private int getBeginningOfLine(string content, int index)
        {
            var start = content.LastIndexOf(Environment.NewLine, index);
            if (start == -1)
                return 0;
            return start + Environment.NewLine.Length;
        }

        private int getEndOfLine(string content, int index)
        {
            var end = content.IndexOf(Environment.NewLine, index);
            if (end == -1)
                return content.Length - 1;
            return end;
        }

        private bool containsLink(string text)
        {
            return text.IndexOf(":line") != -1;
        }

        private string getFile(string text)
        {
            return text.Substring(0, text.IndexOf(":line"));
        }

        private int getLine(string text)
        {
            var start = text.IndexOf(":line") + ":line".Length;
            return int.Parse(text.Substring(start, text.Length - start));
        }

        private void showDetailsWindow(string message, string caption, List<Link> links, int maxWidth)
        {
            if (_testDetails == null)
            {
                _testDetails = new TestDetailsForm((file, line) => goToReference(file, line, 0), goToType);
                _testDetails.Show();
            }
            _testDetails.SetCaption(caption);
            _testDetails.SetText(message, links, maxWidth);
            _testDetails.Visible = true;
            _testDetails.BringToFront();
        }

        private string getTestDetails(CacheTestMessage message)
        {
            var stackTrace = new StringBuilder();
            foreach (var line in message.Test.StackTrace)
            {
                if (File.Exists(line.File))
                {
                    stackTrace.AppendLine(string.Format("at {0} in {1}{2}:line {3}{4}",
                                                        line.Method,
                                                        LinkParser.TAG_START,
                                                        line.File,
                                                        line.LineNumber,
                                                        LinkParser.TAG_END));
                }
                else
                {
                    var text = string.Format("at {0} in {1}{2}{3}",
                                                        line.Method,
                                                        LinkParser.TAG_START,
                                                        line.File,
                                                        LinkParser.TAG_END);
                    if (text.Replace("<<Link>>", "").Replace("<</Link>>", "").Trim() != "at  in")
                        stackTrace.AppendLine(text);
                }

            }
            return string.Format(
                "Assembly: {0}{4}" +
                "Test: {1}{4}{4}" +
                "Message:{4}{2}{4}{4}" +
                "Stack trace{4}{3}",
                message.Assembly,
                message.Test.DisplayName,
                message.Test.Message,
                stackTrace.ToString(),
                Environment.NewLine);
        }

        private string getBuildDetails(CacheBuildMessage message)
        {
            if (File.Exists(Path.Combine(Path.GetDirectoryName(message.Project), message.BuildItem.File)))
            {
                return string.Format(
                    "Project: {0}{6}{6}" +
                    "File: {4}{1}:line {2}{5}{6}{6}" +
                    "Message:{6}{3}",
                    message.Project,
                    message.BuildItem.File,
                    message.BuildItem.LineNumber,
                    message.BuildItem.ErrorMessage,
                    LinkParser.TAG_START,
                    LinkParser.TAG_END,
                    Environment.NewLine);
            }
            else
            {
                return string.Format(
                    "Project: {0}{4}" +
                    "File: {1}:line {2}{4}" +
                    "Message:{4}{3}",
                    message.Project,
                    message.BuildItem.File,
                    message.BuildItem.LineNumber,
                    message.BuildItem.ErrorMessage,
                    Environment.NewLine);
            }
        }

        private void linkLabelSystemMessages_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ShowSystemWindow != null)
                ShowSystemWindow(this, new EventArgs());
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