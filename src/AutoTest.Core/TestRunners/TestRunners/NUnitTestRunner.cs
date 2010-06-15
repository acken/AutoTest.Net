using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using AutoTest.Core.Configuration;
using System.IO;
using System.Diagnostics;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class NUnitTestRunner : ITestRunner
    {
        private readonly string _unitTestExe;
        static readonly ILog _logger = LogManager.GetLogger(typeof(NUnitTestRunner));

        public NUnitTestRunner(IConfiguration configuration)
        {
            _unitTestExe = configuration.MSTestRunner;
        }

        #region ITestRunner Members

        public TestRunResults RunTests(string assemblyName)
        {
            _logger.InfoFormat("Running unit tests using \"{0}\" against assembly {1}.", Path.GetFileName(_unitTestExe), Path.GetFileName(assemblyName));
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(_unitTestExe,
                                                        "\"" + assemblyName + "\"");
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyName);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            string line;
            var testRunResults = new List<TestResult>();
            while ((line = proc.StandardOutput.ReadLine()) != null)
            {
                string detail = String.Empty;

                // "Tests run: 1, Failures: 1, Not run: 0, Time: 0.116 seconds"
                if (line.StartsWith("Tests run:"))
                    Console.WriteLine(line);
                if (line.Contains("]"))
                    detail = line.Substring(line.IndexOf("]") + 1).Trim();

                if (line.StartsWith("[pass") || line.StartsWith("[success"))
                {
                    testRunResults.Add(new TestResult(TestStatus.Passed, detail));
                }
                else if (line.StartsWith("[fail"))
                {
                    testRunResults.Add(new TestResult(TestStatus.Failed, detail));
                }
                else if (line.StartsWith("[ignore"))
                {
                    testRunResults.Add(new TestResult(TestStatus.Ignored, detail));
                }
            }
            proc.WaitForExit();
            return new TestRunResults(testRunResults);
        }

        #endregion
    }
}
