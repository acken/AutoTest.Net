using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using EnvDTE80;
using AutoTest.VS.Util.Debugger;
using EnvDTE;

namespace AutoTest.VSAddin
{
    [ProgId("AutoTestNet_FeedbackWindow"), ClassInterface(ClassInterfaceType.AutoDual), Guid("67663444-f874-401c-9e55-053bb0b5bd0d")]
    public partial class FeedbackWindow : UserControl
    {
        private DTE2 _application;

        public FeedbackWindow()
        {
            InitializeComponent();
        }

        public void SetApplication(DTE2 application)
        {
            _application = application;
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
            var debugger = new DebugHandler(_application);
            debugger.Debug(e.Test);
            Connect.LastDebugRun = e.Test;
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
                var signature = new AutoTest.UI.CodeReflection.TypeConverter(e.Assembly).ToSignature(e.TypeName);
                if (signature != null)
                    AutoTest.VS.Util.MethodFinder_Slow.GotoMethodByFullname(signature, _application);
            }
            catch
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
    }
}
