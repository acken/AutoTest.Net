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
using AutoTest.Core.Messaging.MessageConsumers;

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

        public bool CanHandleTestFor(ProjectDocument document)
        {
            return document.ContainsNUnitTests;
        }

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
			var results = new List<TestRunResults>();
			foreach (var runInfo in runInfos)
			{
	            var unitTestExe = _configuration.NunitTestRunner(runInfo.Project.Value.Framework);
	            if (!File.Exists(unitTestExe))
				{
	                results.Add(new TestRunResults(runInfo.Project.Key, runInfo.Assembly, new TestResult[] {}));
					continue;
				}
				
				var arguments = getExecutableArguments(runInfo.Assembly);
	            var proc = new Process();
	            proc.StartInfo = new ProcessStartInfo(unitTestExe, arguments);
	            proc.StartInfo.RedirectStandardOutput = true;
	            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(runInfo.Assembly);
	            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
	            proc.StartInfo.UseShellExecute = false;
	            proc.StartInfo.CreateNoWindow = true;
	
	            proc.Start();
	            var parser = new NUnitTestResponseParser(_bus);
	            parser.Parse(proc.StandardOutput.ReadToEnd(), runInfos);
	            proc.WaitForExit();
				foreach (var result in parser.Result)
		            results.Add(result);
			}
			return results.ToArray();
        }
        
        string getExecutableArguments (string assemblyName)
		{
			var arguments = "";
			if (System.Environment.OSVersion.Platform.Equals(System.PlatformID.Win32NT) ||
			    System.Environment.OSVersion.Platform.Equals(System.PlatformID.Win32S) ||
			    System.Environment.OSVersion.Platform.Equals(System.PlatformID.Win32Windows) ||
			    System.Environment.OSVersion.Platform.Equals(System.PlatformID.WinCE) ||
			    System.Environment.OSVersion.Platform.Equals(System.PlatformID.Xbox))
			{
				arguments = "/noshadow /xmlconsole \"" + assemblyName + "\"";
			}
			else
			{
				arguments = "--noshadow --xmlconsole \"" + assemblyName + "\"";
			}
			return arguments;
		}

        #endregion
    }
}
