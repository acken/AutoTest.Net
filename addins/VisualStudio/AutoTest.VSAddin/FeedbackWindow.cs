using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AutoTest.Core.Messaging;
using AutoTest.Messages;
using EnvDTE80;
using AutoTest.VS.Util.Debugger;
using EnvDTE;
using AutoTest.VS.Util.Navigation;

namespace AutoTest.VSAddin
{
    [ProgId("AutoTestNet_FeedbackWindow"), ClassInterface(ClassInterfaceType.AutoDual), Guid("67663444-f874-401c-9e55-053bb0b5bd0d")]
    public partial class FeedbackWindow : UserControl
    {
        private DTE2 _application;
        private IMessageBus _bus;

        public event EventHandler<UI.DebugTestArgs> DebugTest;

        public FeedbackWindow()
        {
            InitializeComponent();
        }

        public void SetApplication(DTE2 application)
        {
            _application = application;
        }

        public void SetMessageBus(IMessageBus bus)
        {
            _bus = bus;
        }

        public void Consume(object message)
        {
            runFeedback.ConsumeMessage(message);
        }

        public void SetText(string text)
        {
            runFeedback.PrintMessage(new UI.RunMessages(UI.RunMessageType.Normal, text));
        }

        private void runFeedback_DebugTest(object sender, UI.DebugTestArgs e)
        {
            if (DebugTest != null)
                DebugTest(sender, e);
        }

        private void runFeedback_GoToReference(object sender, UI.GoToReferenceArgs e)
        {
            try
            {
                var window = _application.OpenFile(EnvDTE.Constants.vsViewKindCode, e.Position.File);
                window.Activate();
                var selection = (TextSelection)_application.ActiveDocument.Selection;
                selection.MoveToDisplayColumn(e.Position.LineNumber, e.Position.Column, false);
            }
            catch
            {
            }
        }

        private void runFeedback_GoToType(object sender, UI.GoToTypeArgs e)
        {
            try
            {
                new TypeNavigation().GoToType(_application, e.Assembly, e.TypeName);
            }
            catch (Exception ex)
            {
            }
        }

        public void PrepareForFocus()
        {
            runFeedback.PrepareForFocus();
        }

        public bool IsInFocus()
        {
            return runFeedback.IsInFocus();
        }

        public void Clear()
        {
            runFeedback.ClearList();
        }

        public void ClearBuilds(string project)
        {
            runFeedback.ClearBuilds(project);
        }

        private void FeedbackWindow_Resize(object sender, EventArgs e)
        {
            runFeedback.Resize();
        }

        private void runFeedback_CancelRun(object sender, EventArgs e)
        {
            _bus.Publish(new AbortMessage(""));
        }
    }
}
