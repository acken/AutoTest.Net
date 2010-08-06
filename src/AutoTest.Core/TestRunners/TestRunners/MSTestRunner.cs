using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.Diagnostics;
using System.IO;
using Castle.Core.Logging;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class MSTestRunner : ITestRunner
    {
        private IConfiguration _configuration;

        public MSTestRunner(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region ITestRunner Members

        public bool CanHandleTestFor(ProjectDocument document)
        {
            return document.ContainsMSTests;
        }

        public TestRunResults RunTests(Project project, string assemblyName)
        {
            var timer = Stopwatch.StartNew();
            var unitTestExe = _configuration.MSTestRunner(project.Value.Framework);
            if (!File.Exists(unitTestExe))
                return new TestRunResults(project.Key, assemblyName, new TestResult[] { });

            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(unitTestExe,
                                                        "/testcontainer:\"" + assemblyName + "\" /detail:errorstacktrace /detail:errormessage");
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyName);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            string line;
            var parser = new MSTestResponseParser(project.Key, assemblyName);
            while ((line = proc.StandardOutput.ReadLine()) != null)
            {
                parser.ParseLine(line);
                using (var writer = new StreamWriter("test.log", true))
                {
                    writer.WriteLine(line);
                }
            }
            proc.WaitForExit();
            timer.Stop();
            parser.Result.SetTimeSpent(timer.Elapsed);
            return parser.Result;
        }

        #endregion
    }
}
