using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.Diagnostics;
using System.IO;
using Castle.Core.Logging;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class MSTestRunner : ITestRunner
    {
        private readonly string _unitTestExe;

        public MSTestRunner(IConfiguration configuration)
        {
            _unitTestExe = configuration.MSTestRunner();
        }

        #region ITestRunner Members

        public TestRunResults RunTests(string project, string assemblyName)
        {
            if (!File.Exists(_unitTestExe))
                return new TestRunResults(project, assemblyName, new TestResult[] { });

            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(_unitTestExe,
                                                        "/testcontainer:\"" + assemblyName + "\" /detail:errorstacktrace /detail:errormessage");
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyName);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            string line;
            var parser = new MSTestResponseParser(project, assemblyName);
            while ((line = proc.StandardOutput.ReadLine()) != null)
                parser.ParseLine(line);
            proc.WaitForExit();
            return parser.Result;
        }

        #endregion
    }
}
