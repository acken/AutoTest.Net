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

        public bool CanHandleTestFor(ProjectDocument document)
        {
            return document.ContainsNUnitTests;
        }

        public TestRunResults RunTests(Project project, string assemblyName)
        {
            var timer = Stopwatch.StartNew();
            var unitTestExe = _configuration.NunitTestRunner(project.Value.Framework);
            if (!File.Exists(unitTestExe))
                return new TestRunResults(project.Key, assemblyName, new TestResult[] {});
			
			var arguments = getExecutableArguments(assemblyName);
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo(unitTestExe, arguments);
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyName);
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            var parser = new NUnitTestResponseParser(_bus, project.Key, assemblyName);
            parser.Parse(proc.StandardOutput.ReadToEnd());
            proc.WaitForExit();
            timer.Stop();
            var result = parser.Result;
            result.SetTimeSpent(timer.Elapsed);
            return result;
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
