using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AutoTest.Core.Configuration;

namespace AutoTest.WinForms
{
    public partial class WatchDirectoryPickerForm : Form, IWatchDirectoryPicker
    {
        private IConfiguration _configuration;
        private string _directoryToWatch = "";

        public string DirectoryToWatch { get { return _directoryToWatch; } }

        public WatchDirectoryPickerForm(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeComponent();
            addWatchDirectoriesFromConfiguration();
        }

        private void addWatchDirectoriesFromConfiguration()
        {
            foreach (var directory in _configuration.WatchDirectores)
                listViewDirectories.Items.Add(directory);
			if (listViewDirectories.Items.Count > 0)
				listViewDirectories.Items[0].Selected = true;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var browse = new FolderBrowserDialog();
            if (browse.ShowDialog(this) == DialogResult.OK)
                textBoxDirectory.Text = browse.SelectedPath;
        }

        private void listViewDirectories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewDirectories.SelectedItems.Count > 0)
                textBoxDirectory.Text = listViewDirectories.SelectedItems[0].Text;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            var directory = textBoxDirectory.Text.Trim();
            if (!Directory.Exists(directory))
            {
                showInvalidDirectoryError();
                return;
            }
            _directoryToWatch = directory;
            DialogResult = DialogResult.OK;
            Close();
        }

        private static void showInvalidDirectoryError()
        {
            MessageBox.Show("The directory you specified does not exist. Please choose a valid directory.",
                "Watch directory selection",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
