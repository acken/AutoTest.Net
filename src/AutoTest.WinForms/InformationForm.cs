using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;

namespace AutoTest.WinForms
{
    public partial class InformationForm : Form, IInformationFeedbackView, IInformationForm
    {
        private SynchronizationContext _syncContext;
        private IInformationFeedbackPresenter _presenter;

        private int _rightSpacing = 0;
        private int _listBottomSpacing = 0;
        private int _infoBottomSpacing = 0;

        public event EventHandler<MessageRecievedEventArgs> MessageArrived;

        public InformationForm(IInformationFeedbackPresenter presenter)
        {
            _syncContext = AsyncOperationManager.SynchronizationContext;
            _presenter = presenter;
            _presenter.View = this;
            InitializeComponent();
            readFormSpacing();
			InformationForm_Resize(this, new EventArgs());
        }

        private void readFormSpacing()
        {
            _rightSpacing = Width - (informationList.Left + informationList.Width);
            _listBottomSpacing = linkLabelInfo.Top - (informationList.Top + informationList.Height);
            _infoBottomSpacing = Height - (linkLabelInfo.Top + linkLabelInfo.Height);
        }

        #region IInformationFeedbackView Members

        public void RecievingInformationMessage(InformationMessage message)
        {
            _syncContext.Post(m =>
                                  {
                                      addToList(m);
                                      if (MessageArrived != null)
                                          MessageArrived(this, new MessageRecievedEventArgs(MessageType.Information));
                                  }, message.Message);
        }

        public void RecievingWarningMessage(WarningMessage message)
        {
            _syncContext.Post(m =>
                                  {
                                      addToList(m);
                                      if (MessageArrived != null)
                                          MessageArrived(this, new MessageRecievedEventArgs(MessageType.Warning));
                                  }, message.Warning);
        }

        public void RevievingErrorMessage(ErrorMessage message)
        {
            _syncContext.Post(m =>
                                  {
                                      addToList(m);
                                      if (MessageArrived != null)
                                          MessageArrived(this, new MessageRecievedEventArgs(MessageType.Error));
                                  }, message.Error);
        }

        #endregion

        private void setInfoText(string text)
        {
            int previousHeight = linkLabelInfo.Height;
            linkLabelInfo.Text = text;
            linkLabelInfo.LinkArea = new LinkArea(0, 0);
            var difference = linkLabelInfo.Height - previousHeight;
            Height = Height + difference;
        }

        private void addToList(object message)
        {
            var text = message.ToString();
            if (text.Length > 150)
                text = string.Format("{0}...", text.Substring(0, 150));
            var item = informationList.Items.Add(text);
            item.Tag = message.ToString();
        }

        private void informationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (informationList.SelectedItems.Count != 1)
            {
                setInfoText("");
                return;
            }
            setInfoText(informationList.Items[informationList.SelectedItems[0].Index].Tag.ToString());
        }

        private void InformationForm_Resize(object sender, EventArgs e)
        {
            linkLabelInfo.MaximumSize = new Size(Width - (linkLabelInfo.Left + _rightSpacing), 0);
			// This is truely horrendous but it does the job for now
			if (Environment.OSVersion.Platform.Equals(PlatformID.Unix))
			{
				linkLabelInfo.Top = Height - (linkLabelInfo.Height + _infoBottomSpacing + 100);
				informationList.Height = linkLabelInfo.Top - (informationList.Top + _listBottomSpacing);
            	informationList.Width = Width - (informationList.Left + _rightSpacing + 10);
			}
			else
			{
				linkLabelInfo.Top = Height - (linkLabelInfo.Height + _infoBottomSpacing);
            	informationList.Height = linkLabelInfo.Top - (informationList.Top + _listBottomSpacing);
            	informationList.Width = Width - (informationList.Left + _rightSpacing);
			}
        }

        #region IInformationForm Members

        public Form Form
        {
            get { return this; }
        }

        #endregion

        private void InformationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }
    }
}
