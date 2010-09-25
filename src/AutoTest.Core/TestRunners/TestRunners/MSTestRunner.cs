using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using System.Diagnostics;
using System.IO;
using Castle.Core.Logging;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging.MessageConsumers;

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

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
			var results = new List<TestRunResults>();
			foreach (var runInfo in runInfos)
			{			
	            var timer = Stopwatch.StartNew();
	            var unitTestExe = _configuration.MSTestRunner(runInfo.Project.Value.Framework);
	            if (!File.Exists(unitTestExe))
				{
	                results.Add(new TestRunResults(runInfo.Project.Key, runInfo.Assembly, new TestResult[] { }));
					continue;
				}
	
	            var proc = new Process();
	            proc.StartInfo = new ProcessStartInfo(unitTestExe,
	                                                        "/testcontainer:\"" + runInfo.Assembly + "\" /detail:errorstacktrace /detail:errormessage");
	            proc.StartInfo.RedirectStandardOutput = true;
	            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(runInfo.Assembly);
	            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
	            proc.StartInfo.UseShellExecute = false;
	            proc.StartInfo.CreateNoWindow = true;
	
	            proc.Start();
	            string line;
	            var parser = new MSTestResponseParser(runInfo.Project.Key, runInfo.Assembly);
	            while ((line = proc.StandardOutput.ReadLine()) != null)
	                parser.ParseLine(line);
	            proc.WaitForExit();
	            timer.Stop();
	            parser.Result.SetTimeSpent(timer.Elapsed);
	            results.Add(parser.Result);
			}
			return results.ToArray();
        }

        #endregion
    }
}
