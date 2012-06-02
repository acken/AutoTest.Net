using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AutoTest.Messages;

namespace AutoTest.UI
{
    class ListItemBehaviourHandler : IDisposable
    {
        private RunFeedback _control;
        private List<KeyValuePair<string, Control>> _controls = new List<KeyValuePair<string, Control>>();

        public ListItemBehaviourHandler(RunFeedback control)
        {
            _control = control;
            addControl(_control.linkLabelCancelRun);
            if (control.CanDebug)
                addControl(_control.linkLabelDebugTest);
            addControl(_control.linkLabelTestDetails);
            addControl(_control.linkLabelErrorDescription);
        }

        private void addControl(Control control)
        {
            _controls.Add(new KeyValuePair<string, Control>(control.Name, control));
        }

        public void Organize(ListView.SelectedListViewItemCollection selectedItems, bool isRunning)
        {
            if (selectedItems == null)
                onNothing(isRunning);
            else if (selectedItems.Count != 1)
                onNothing(isRunning);
            else if (selectedItems[0].Tag.GetType() == typeof(CacheBuildMessage))
                onBuildMessage((CacheBuildMessage)selectedItems[0].Tag, isRunning);
            else if (selectedItems[0].Tag.GetType() == typeof(CacheTestMessage))
                onTestMessage((CacheTestMessage)selectedItems[0].Tag, isRunning);
        }

        private void onBuildMessage(CacheBuildMessage cacheBuildMessage, bool isRunning)
        {
            if (isRunning)
                displayAndOrder(new string[] { _control.linkLabelErrorDescription.Name, _control.linkLabelCancelRun.Name });
            else
                displayAndOrder(new string[] { _control.linkLabelErrorDescription.Name });
            
        }

        private void onTestMessage(CacheTestMessage cacheTestMessage, bool isRunning)
        {
            var controls = new List<string>();
            controls.Add(_control.linkLabelTestDetails.Name);
            controls.Add(_control.linkLabelDebugTest.Name);
            if (isRunning)
                controls.Add(_control.linkLabelCancelRun.Name);
            displayAndOrder(controls.ToArray());
        }

        private void onNothing(bool isRunning)
        {
            if (isRunning)
                displayAndOrder(new string[] { _control.linkLabelCancelRun.Name });
            else
                displayAndOrder(new string[] { });
        }

        private void displayAndOrder(string[] controlsToShow)
        {
            var nextControlPosition = _control.Width - 5;
            foreach (var control in _controls)
            {
                if (controlsToShow.Contains(control.Key))
                {
                    var item = control.Value;
                    item.Left = nextControlPosition - item.Width;
                    nextControlPosition = item.Left - 5;
                    item.Visible = true;
                    continue;
                }
                control.Value.Visible = false;
            }
        }

        public void Dispose()
        {
            _controls = null;
            _control = null;
        }
    }
}
