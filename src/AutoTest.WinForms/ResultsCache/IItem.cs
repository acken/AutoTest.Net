using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.WinForms.ResultsCache
{
    interface IItem
    {
        string ToString();
        void HandleLink(string link);
    }
}
