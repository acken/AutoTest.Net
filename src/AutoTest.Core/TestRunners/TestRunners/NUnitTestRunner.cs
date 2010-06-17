using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.IO;
using System.Diagnostics;
using Castle.Core.Logging;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class NUnitTestRunner : ITestRunner
    {
        private readonly string _unitTestExe;
        private ILogger _logger;

        public ILogger Logger
        {
            get { if (_logger == null) _logger = NullLogger.Instance; return _logger; }
            set { _logger = value; }
        }

        public NUnitTestRunner(IConfiguration configuration)
        {
            _unitTestExe = configuration.NunitTestRunner;
        }

        #region ITestRunner Members

        public TestRunResults RunTests(string assemblyName)
        {
            Logger.InfoFormat("Running unit tests using \"{0}\" against assembly {1}.", Path.GetFileName(_unitTestExe), Path.GetFileName(assemblyName));
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(_unitTestExe,
                                                        "/noshadow /xmlconsole \"" + assemblyName + "\"");
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyName);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            string line;
            var parser = new NUnitTestResponseParser(_logger);
            parser.Parse(proc.StandardOutput.ReadToEnd());
            proc.WaitForExit();
            return parser.Result;
        }

        #endregion
    }
}
