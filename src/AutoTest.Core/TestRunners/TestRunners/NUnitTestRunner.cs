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
			// Get a list of the various nunit executables specified pr. framework version
			var nUnitExes = getNUnitExes(runInfos);
			foreach (var nUnitExe in nUnitExes)
			{
				// Get the assemblies that should be run under this nunit executable
				var assemblies = getAssembliesFromTestRunner(nUnitExe, runInfos);
				var arguments = getExecutableArguments(assemblies);
	            var proc = new Process();
	            proc.StartInfo = new ProcessStartInfo(nUnitExe, arguments);
	            proc.StartInfo.RedirectStandardOutput = true;
	            //proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(runInfo.Assembly);
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
		
		private string[] getNUnitExes(TestRunInfo[] runInfos)
		{
			var testRunnerExes = new List<string>();
			foreach (var runInfo in runInfos)
			{
				var unitTestExe = _configuration.NunitTestRunner(runInfo.Project.Value.Framework);
	            if (File.Exists(unitTestExe))
				{
					if (!testRunnerExes.Exists(x => x.Equals(unitTestExe)))
	                	testRunnerExes.Add(unitTestExe);
				}
			}
			return testRunnerExes.ToArray();
		}
		
		private string getAssembliesFromTestRunner(string testRunnerExes, TestRunInfo[] runInfos)
		{
			var assemblies = "";
			foreach (var runInfo in runInfos)
			{
				var unitTestExe = _configuration.NunitTestRunner(runInfo.Project.Value.Framework);
				if (unitTestExe.Equals(testRunnerExes))
					assemblies += string.Format("\"{0}\"", runInfo.Assembly) + " ";
			}
			return assemblies;
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
				arguments = "/noshadow /xmlconsole " + assemblyName;
			}
			else
			{
				arguments = "--noshadow --xmlconsole " + assemblyName;
			}
			return arguments;
		}

        #endregion
    }
}
