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
        private ILogger _logger;

        public ILogger Logger
        {
            get { if (_logger == null) _logger = NullLogger.Instance; return _logger; }
            set { _logger = value; }
        }

        public MSTestRunner(IConfiguration configuration)
        {
            _unitTestExe = configuration.MSTestRunner;
        }

        #region ITestRunner Members

        public TestRunResults RunTests(string assemblyName)
        {
            Logger.InfoFormat("Running unit tests using \"{0}\" against assembly {1}.", Path.GetFileName(_unitTestExe), Path.GetFileName(assemblyName));
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(_unitTestExe,
                                                        "/testcontainer:\"" + assemblyName + "\"");
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyName);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            string line;
            var parser = new MSTestResponseParser();
            while ((line = proc.StandardOutput.ReadLine()) != null)
                parser.ParseLine(line);
            proc.WaitForExit();
            return parser.Result;
        }

        #endregion
    }
}
