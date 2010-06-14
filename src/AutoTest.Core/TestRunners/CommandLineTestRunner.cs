using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using log4net;
using AutoTest.Core.Configuration;

namespace AutoTest.Core.TestRunners
{
    public class CommandLineTestRunner : ITestRunner
    {
        readonly string _unitTestExe;
        static readonly ILog _logger = LogManager.GetLogger(typeof(CommandLineTestRunner));

        public CommandLineTestRunner(IConfiguration configuration)
        {
            _unitTestExe = configuration.UnitTestExe;
        }

        public TestRunResults RunTests(string assemblyName)
        {
            _logger.InfoFormat("Running unit tests using \"{0}\" against assembly {1}.", Path.GetFileName(_unitTestExe), Path.GetFileName(assemblyName));
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
                    Console.WriteLine(line);
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
            return new TestRunResults(results);
        }
    }
}