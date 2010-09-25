using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Configuration;
using System.IO;
using System.Diagnostics;
using AutoTest.Core.Messaging;
using AutoTest.Core.Messaging.MessageConsumers;

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

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
			var results = new List<TestRunResults>();
			foreach (var runInfo in runInfos)
			{
	            var unitTestExe = _configuration.XunitTestRunner(runInfo.Project.Value.Framework);
	            if (!File.Exists(unitTestExe))
				{
	                results.Add(new TestRunResults(runInfo.Project.Key, runInfo.Assembly, new TestResult[] { }));
					continue;
				}
	
	            var resultFile = Path.GetTempFileName();
	            var arguments = string.Format("\"{0}\" /noshadow /nunit \"{1}\"", runInfo.Assembly, resultFile);
	            var proc = new Process();
	            proc.StartInfo = new ProcessStartInfo(unitTestExe, arguments);
	            proc.StartInfo.RedirectStandardOutput = true;
	            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(runInfo.Assembly);
	            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
	            proc.StartInfo.UseShellExecute = false;
	            proc.StartInfo.CreateNoWindow = true;
	
	            proc.Start();
	            // Make sure we empty buffer
	            proc.StandardOutput.ReadToEnd();
	            proc.WaitForExit();
	            var parser = new NUnitTestResponseParser(_bus);
	            using (TextReader reader = new StreamReader(resultFile))
	            {
					var fileContent = reader.ReadToEnd(); 
	                parser.Parse(fileContent, runInfos);
					Console.WriteLine(fileContent);
	            }
	            File.Delete(resultFile);
				foreach (var result in parser.Result)
		            results.Add(result);
			}
			return results.ToArray();
        }

        #endregion
    }
}
