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
using AutoTest.Core.FileSystem;
using AutoTest.Messages;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class NUnitTestRunner : ITestRunner
    {
        private IMessageBus _bus;
        private IConfiguration _configuration;
		private IResolveAssemblyReferences _referenceResolver;

        public NUnitTestRunner(IMessageBus bus, IConfiguration configuration, IResolveAssemblyReferences referenceResolver)
        {
            _bus = bus;
            _configuration = configuration;
			_referenceResolver = referenceResolver;
        }

        #region ITestRunner Members

        public bool CanHandleTestFor(ProjectDocument document)
        {
            return document.ContainsNUnitTests;
        }
		
		public bool CanHandleTestFor(string assembly)
		{
			var references = _referenceResolver.GetReferences(assembly);
			return references.Contains("nunit.framework");
		}

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
			var results = new List<TestRunResults>();
			// Get a list of the various nunit executables specified pr. framework version
			var nUnitExes = getNUnitExes(runInfos);
			foreach (var nUnitExe in nUnitExes)
			{
				// Get the assemblies that should be run under this nunit executable
				var assemblies = getAssembliesAndTestsForTestRunner(nUnitExe.Exe, runInfos);
                if (assemblies == null)
                    continue;
				var arguments = getExecutableArguments(nUnitExe, assemblies, runInfos);
				DebugLog.Debug.WriteMessage(string.Format("Running tests: {0} {1}", nUnitExe.Exe, arguments)); 
	            var proc = new Process();
	            proc.StartInfo = new ProcessStartInfo(nUnitExe.Exe, arguments);
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
		
		private RunnerExe[] getNUnitExes(TestRunInfo[] runInfos)
		{
			var testRunnerExes = new List<RunnerExe>();
			foreach (var runInfo in runInfos)
			{
				var framework = getFramework(runInfo);
				var unitTestExe = _configuration.NunitTestRunner(framework);
				if (_configuration.GetSpesificNunitTestRunner(framework) == null)
					framework = "";
	            if (File.Exists(unitTestExe))
				{
					if (!testRunnerExes.Exists(x => x.Equals(new RunnerExe(unitTestExe, framework))))
	                	testRunnerExes.Add(new RunnerExe(unitTestExe, framework));
				}
			}
			return testRunnerExes.ToArray();
		}
		
		private string getFramework(TestRunInfo runInfo)
		{
			if (runInfo.Project == null)
				return "";
			return runInfo.Project.Value.Framework;
		}
		
		private string getAssembliesAndTestsForTestRunner(string testRunnerExes, TestRunInfo[] runInfos)
		{
			var separator = getArgumentSeparator();
			var assemblies = "";
			var tests = "";
			foreach (var runInfo in runInfos)
			{
				var unitTestExe = _configuration.NunitTestRunner(getFramework(runInfo));
				if (unitTestExe.Equals(testRunnerExes))
				{
                    if (runInfo.OnlyRunSpcifiedTests && runInfo.TestsToRun.Length.Equals(0))
                        continue;
					assemblies += string.Format("\"{0}\"", runInfo.Assembly) + " ";
					var assemblyTests = getTestsList(runInfo);
					if (assemblyTests.Length > 0)
						tests += (tests.Length > 0 ? "," : "") + assemblyTests;
				}
			}
			if (tests.Length > 0)
				tests = string.Format("{0}run={1}", separator, tests);
            if (assemblies.Length.Equals(0))
                return null;
			return string.Format("{0} {1}", tests, assemblies);
		}
        
        string getExecutableArguments (RunnerExe exe, string assemblyName, TestRunInfo[] runInfos)
		{
			var separator = getArgumentSeparator();
			string framework = "";
			// only use framework for windows as the default runner on linux has no framework parameter
			if (!Environment.OSVersion.Platform.Equals(PlatformID.Unix) && !Environment.OSVersion.Platform.Equals(PlatformID.MacOSX))
			{
				if (exe.Version.Length > 0)
					framework = string.Format(" {0}framework:{1}", separator, exe.Version);
			}
			var categoryList = getCategoryIgnoreList();
			return string.Format("{0}noshadow{2} {0}xmlconsole {1}", separator, categoryList, framework) + assemblyName;
		}

        #endregion
		
		private string getTestsList(TestRunInfo runInfo)
		{
			var tests = "";
			foreach (var test in runInfo.TestsToRun)
				tests += (tests.Length > 0 ? "," : "") + test;
			return tests;
		}
		
		private string getCategoryIgnoreList()
		{
			var separator = getArgumentSeparator();
			string categoryList = "";
			foreach (var category in _configuration.TestCategoriesToIgnore)
			{
				categoryList += (categoryList.Length > 0 ? "," : "") + category;
			}
			if (categoryList.Length > 0)
				categoryList = separator + "exclude=" + categoryList + " ";
			return categoryList;
		}
		
		private string getArgumentSeparator()
		{
			if (System.Environment.OSVersion.Platform.Equals(System.PlatformID.Win32NT) ||
			    System.Environment.OSVersion.Platform.Equals(System.PlatformID.Win32S) ||
			    System.Environment.OSVersion.Platform.Equals(System.PlatformID.Win32Windows) ||
			    System.Environment.OSVersion.Platform.Equals(System.PlatformID.WinCE) ||
			    System.Environment.OSVersion.Platform.Equals(System.PlatformID.Xbox))
			{
				return "/";
			}
			else
			{
				return "--";
			}
		}
    }
}
