using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using AutoTest.Core.Configuration;
using System.Diagnostics;
using System.IO;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class MSTestRunner : ITestRunner
    {
        private readonly string _unitTestExe;
        static readonly ILog _logger = LogManager.GetLogger(typeof(MSTestRunner));

        public MSTestRunner(IConfiguration configuration)
        {
            _unitTestExe = configuration.MSTestRunner;
        }

        #region ITestRunner Members

        public TestRunResults RunTests(string assemblyName)
        {
            _logger.InfoFormat("Running unit tests using \"{0}\" against assembly {1}.", Path.GetFileName(_unitTestExe), Path.GetFileName(assemblyName));
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
