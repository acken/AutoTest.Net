using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;

namespace AutoTest.VSAddin
{
    public partial class InformationWindow : Form, IInformationFeedbackView
    {
        private IInformationFeedbackPresenter _presenter;

        public InformationWindow(IInformationFeedbackPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.View = this;
        }

        public void RecievingInformationMessage(InformationMessage message)
        {
            listViewInformation.Items.Add(message.Message);
        }

        public void RecievingWarningMessage(WarningMessage message)
        {
            listViewInformation.Items.Add(message.Warning);
        }

        public void RevievingErrorMessage(ErrorMessage message)
        {
            listViewInformation.Items.Add(message.Error);
        }

        private void InformationWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }
    }
}
