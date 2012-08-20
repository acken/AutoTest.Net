using System;
using System.Windows.Forms;
using AutoTest.Core.Launchers;
using AutoTest.Core.Messaging;
using AutoTest.Messages;
using AutoTest.Core.DebugLog;

namespace AutoTest.WinForms
{
    public partial class FeedbackForm : Form, IOverviewForm, IMessageForwarder
    {
        private IApplicatonLauncher _launcher;
        private IMessageBus _bus;
        private string _watchToken = null;

        public FeedbackForm(IApplicatonLauncher launcher, IMessageBus bus)
        {
            _launcher = launcher;
            _bus = bus;
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
            Text = "AutoTest.Net - " + _watchToken;
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
				Debug.WriteDebug("Handling external messag: " + commandMessage.Command + " from " + commandMessage.Sender);
                if (commandMessage.Sender == "EditorEngine")
                {
                    var msg = EditorEngineMessage.New(commandMessage.Sender + " " + commandMessage.Command);
					Debug.WriteDebug(" args count is " + msg.Arguments.Count.ToString());
					if (msg.Arguments.Count == 1 &&
						msg.Arguments[0].ToLower() == "shutdown")
					{
						Debug.WriteDebug("Shutting down");
						Close();
					}
                    if (msg.Arguments.Count == 2 &&
						msg.Arguments[0].ToLower() == "autotest.net" &&
						msg.Arguments[1].ToLower() == "setfocus")
                    {
                        Activate();
                        runFeedback.PrepareForFocus();
                    }
                }
            }
        }

        private void runFeedback_CancelRun(object sender, EventArgs e)
        {
            _bus.Publish(new AbortMessage(""));
        }
    }
}
