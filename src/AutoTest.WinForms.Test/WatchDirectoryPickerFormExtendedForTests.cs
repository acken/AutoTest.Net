using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.Windows.Forms;

namespace AutoTest.WinForms.Test
{
    class WatchDirectoryPickerFormExtendedForTests : WatchDirectoryPickerForm
    {
        public WatchDirectoryPickerFormExtendedForTests(IConfiguration configuration)
            : base(configuration)
        {
        }

        public string[] GetListViewContent()
        {
            var items = new List<string>();
            foreach (ListViewItem item in listViewDirectories.Items)
                items.Add(item.Text);
            return items.ToArray();
        }
    }
}
