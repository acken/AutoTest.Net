using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.IO;
using System.Diagnostics;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class NUnitTestRunner : ITestRunner
    {
        private IMessageBus _bus;
        private IConfiguration _configuration;

        public NUnitTestRunner(IMessageBus bus, IConfiguration configuration)
        {
            _bus = bus;
            _configuration = configuration;
        }

        #region ITestRunner Members

        public TestRunResults RunTests(Project project, string assemblyName)
        {
            var unitTestExe = _configuration.NunitTestRunner(project.Value.Framework);
            if (!File.Exists(unitTestExe))
                return new TestRunResults(project.Key, assemblyName, new TestResult[] {});

            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(unitTestExe,
                                                        "/noshadow /xmlconsole \"" + assemblyName + "\"");
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyName);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            var parser = new NUnitTestResponseParser(_bus, project.Key, assemblyName);
            parser.Parse(proc.StandardOutput.ReadToEnd());
            proc.WaitForExit();
            return parser.Result;
        }

        #endregion
    }
}
