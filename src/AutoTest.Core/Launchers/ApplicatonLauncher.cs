using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using AutoTest.Core.Configuration;

namespace AutoTest.Core.Launchers
{
    public class ApplicatonLauncher
    {
        private IConfiguration _configuration;

        public ApplicatonLauncher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void LaunchEditor(string file, int lineNumber)
        {
            var executable = _configuration.CodeEditor.Executable;
            var arguments = _configuration.CodeEditor.Arguments;
            arguments = arguments.Replace("[[CodeFile]]", file);
            arguments = arguments.Replace("[[LineNumber]]", lineNumber.ToString());
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(executable, arguments);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
        }
    }
}
