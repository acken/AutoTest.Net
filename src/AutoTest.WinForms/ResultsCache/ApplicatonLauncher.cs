using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoTest.WinForms.ResultsCache
{
    class ApplicatonLauncher
    {
        private string _file;
        private int _lineNumber;

        public ApplicatonLauncher(string file, int lineNumber)
        {
            _file = file;
            _lineNumber = lineNumber;
        }

        public void Launch()
        {
            var executable = @"C:\Program Files (x86)\Microsoft Visual Studio 9.0\Common7\IDE\devenv";
            var arguments = string.Format("/Edit \"{0}\" /command \"Edit.Goto {1}\"", _file, _lineNumber);
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(executable, arguments);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
        }
    }
}
