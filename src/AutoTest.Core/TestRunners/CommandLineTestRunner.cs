using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AutoTest.Core.Configuration;
using Castle.Core.Logging;

namespace AutoTest.Core.TestRunners
{
    public class CommandLineTestRunner : ITestRunner
    {
        private readonly string _unitTestExe;
        private ILogger _logger;

        public ILogger Logger
        {
            get { if (_logger == null) _logger = NullLogger.Instance; return _logger; }
            set { _logger = value; }
        }

        public CommandLineTestRunner(IConfiguration configuration)
        {
            _unitTestExe = configuration.NunitTestRunner;
        }

        public TestRunResults RunTests(string assemblyName)
        {
            Logger.InfoFormat("Running unit tests using \"{0}\" against assembly {1}.", Path.GetFileName(_unitTestExe), Path.GetFileName(assemblyName));
            ProcessStartInfo psi = new ProcessStartInfo(_unitTestExe,
                                                        "\"" + assemblyName + "\"");
            psi.WorkingDirectory = Path.GetDirectoryName(_unitTestExe);
            psi.RedirectStandardOutput = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process proc = Process.Start(psi);

            proc.WaitForExit();

            var results = new List<TestResult>();
            while(!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                string detail = String.Empty;

                // "Tests run: 1, Failures: 1, Not run: 0, Time: 0.116 seconds"
                if (line.StartsWith("Tests run:"))
                    Logger.Info(line);
                if(line.Contains("]"))
                    detail = line.Substring(line.IndexOf("]") + 1).Trim();

                if(line.StartsWith("[pass") || line.StartsWith("[success"))
                {
                    results.Add(new TestResult(TestStatus.Passed, detail));
                }
                else if(line.StartsWith("[fail"))
                {
                    results.Add(new TestResult(TestStatus.Failed, detail));
                }
                else if(line.StartsWith("[ignore"))
                {
                    results.Add(new TestResult(TestStatus.Ignored, detail));
                }
            }
            return new TestRunResults(results.ToArray());
        }
    }
}