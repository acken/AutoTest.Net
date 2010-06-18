using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.IO;
using System.Diagnostics;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class NUnitTestRunner : ITestRunner
    {
        private IMessageBus _bus;
        private readonly string _unitTestExe;

        public NUnitTestRunner(IMessageBus bus, IConfiguration configuration)
        {
            _bus = bus;
            _unitTestExe = configuration.NunitTestRunner;
        }

        #region ITestRunner Members

        public TestRunResults RunTests(string project, string assemblyName)
        {
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(_unitTestExe,
                                                        "/noshadow /xmlconsole \"" + assemblyName + "\"");
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyName);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            var parser = new NUnitTestResponseParser(_bus, project, assemblyName);
            parser.Parse(proc.StandardOutput.ReadToEnd());
            proc.WaitForExit();
            return parser.Result;
        }

        #endregion
    }
}
