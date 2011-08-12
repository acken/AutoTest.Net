using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoTest.WinForms
{
    public interface IOverviewForm
    {
        void SetWatchDirectory(string directory);
        FeedbackForm Form { get; }
    }
}
