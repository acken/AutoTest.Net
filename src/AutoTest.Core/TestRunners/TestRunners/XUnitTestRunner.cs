using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Configuration;
using System.IO;
using System.Diagnostics;
using AutoTest.Core.Messaging;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class XUnitTestRunner : ITestRunner
    {
        private IMessageBus _bus;
        private IConfiguration _configuration;

        public XUnitTestRunner(IMessageBus bus, IConfiguration configuration)
        {
            _bus = bus;
            _configuration = configuration;
        }

        #region ITestRunner Members

        public bool CanHandleTestFor(ProjectDocument document)
        {
            return document.ContainsXUnitTests;
        }

        public TestRunResults RunTests(Project project, string assemblyName)
        {
            var timer = Stopwatch.StartNew();
            var unitTestExe = _configuration.XunitTestRunner(project.Value.Framework);
            if (!File.Exists(unitTestExe))
                return new TestRunResults(project.Key, assemblyName, new TestResult[] { });

            var resultFile = Path.GetTempFileName();
            var arguments = string.Format("\"{0}\" /noshadow /nunit \"{1}\"", assemblyName, resultFile);
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(unitTestExe, arguments);
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyName);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            // Make sure we empty buffer
            proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            timer.Stop();
            var parser = new NUnitTestResponseParser(_bus, project.Key, assemblyName);
            using (TextReader reader = new StreamReader(resultFile))
            {
                parser.Parse(reader.ReadToEnd());
            }
            File.Delete(resultFile);
            var result = parser.Result;
            result.SetTimeSpent(timer.Elapsed);
            return result;
        }

        #endregion
    }
}
