using System;
using System.Windows.Forms;
using AutoTest.Core.Launchers;
using AutoTest.Messages;
using AutoTest.Core.DebugLog;

namespace AutoTest.WinForms
{
    public partial class FeedbackForm : Form, IOverviewForm, IMessageForwarder
    {
        private IApplicatonLauncher _launcher;
        private string _watchToken = null;

        public FeedbackForm(IApplicatonLauncher launcher)
        {
            _launcher = launcher;
            InitializeComponent();
            runFeedback.PrintMessage(new UI.RunMessages(UI.RunMessageType.Normal, "Listening for changes"));
        }

        private void runFeedback_GoToReference(object sender, UI.GoToReferenceArgs e)
        {
            _launcher.LaunchEditor(e.Position.File, e.Position.LineNumber, e.Position.Column);
        }

        public void SetWatchDirectory(string directory)
        {
            _watchToken = directory;
        }
        
        public FeedbackForm Form
        {
            get { return this; }
        }

        public void Forward(object message)
        {
            Debug.WriteDetail("Handling {0}", message.GetType());
            runFeedback.ConsumeMessage(message);
            if (message.GetType().Equals(typeof(ExternalCommandMessage)))
            {
                var commandMessage = (ExternalCommandMessage)message;
                if (commandMessage.Sender == "EditorEngine")
                {
                    var msg = EditorEngineMessage.New(commandMessage.Command);
                    if (msg.Arguments.Count == 2 &&
						msg.Arguments[0].ToLower() == "autotest.net" &&
						msg.Arguments[0].ToLower() == "setfocus")
                    {
                        Activate();
                        runFeedback.PrepareForFocus();
                    }
                }
            }
        }
    }
}
