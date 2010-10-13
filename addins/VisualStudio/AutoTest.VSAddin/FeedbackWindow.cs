using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using AutoTest.Core.Caching.RunResultCache;
using System.Drawing;
using EnvDTE80;

namespace AutoTest.VSAddin
{
    [ProgId("AutoTestFeedbackWindow"), ClassInterface(ClassInterfaceType.AutoDual), Guid("1C96720B-163C-41EF-9E00-1FC3731CF613")]
    public partial class FeedbackWindow : UserControl
    {
        private FeedbackView _viewHandler;
        private SynchronizationContext _syncContext;

        public FeedbackWindow()
        {
            _syncContext = AsyncOperationManager.SynchronizationContext;
            InitializeComponent();
        }

        internal void CreateViewHandler(DTE2 application)
        {
            _viewHandler = new FeedbackView(x => printMessage(x), x => relistCache(x), application);
        }

        private void printMessage(RunMessages message)
        {
            _syncContext.Post(s => 
                {
                    var msg = (RunMessages)s;
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
                    this.Refresh();
                }, message);
        }

        private void relistCache(IRunResultCache cache)
        {
            _syncContext.Post(c => rePopulateList(c), cache);
        }

        private void rePopulateList(object cacheObject)
        {
            IRunResultCache cache = (IRunResultCache)cacheObject;
            IItem selected = null;
            if (listViewFeedback.SelectedItems.Count == 1)
                selected = (IItem)listViewFeedback.SelectedItems[0].Tag;

            listViewFeedback.Items.Clear();
            foreach (var error in cache.Errors)
                addFeedbackItem("Build error", formatBuildResult(error), Color.Red, error, selected);

            foreach (var failed in cache.Failed)
                addFeedbackItem("Test failed", formatTestResult(failed), Color.Red, failed, selected);

            foreach (var warning in cache.Warnings)
                addFeedbackItem("Build warning", formatBuildResult(warning), Color.Black, warning, selected);

            foreach (var ignored in cache.Ignored)
                addFeedbackItem("Test ignored", formatTestResult(ignored), Color.Black, ignored, selected);
            this.Refresh();
        }

        private void addFeedbackItem(string type, string message, Color colour, IItem tag, IItem selected)
        {
            var item = listViewFeedback.Items.Add(type);
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
            return string.Format("{0} -> {1}", test.Status, test.Name);
        }

        private void buttonInformation_Click(object sender, EventArgs e)
        {
            _viewHandler.ShowInformation();
        }

        private void listViewFeedback_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewFeedback.SelectedItems.Count != 1)
                return;

            _viewHandler.GoToMessageReference((IItem)listViewFeedback.SelectedItems[0].Tag);
        }

        private void listViewFeedback_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            if (listViewFeedback.SelectedItems.Count != 1)
                return;
            _viewHandler.ShowMessageInfo((IItem)listViewFeedback.SelectedItems[0].Tag, Cursor.Position);
        }
    }
}
