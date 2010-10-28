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
		private IPreProcessTestruns[] _preProcessors;

        public NUnitTestRunner(IMessageBus bus, IConfiguration configuration, IResolveAssemblyReferences referenceResolver, IPreProcessTestruns[] preProcessors)
        {
            _bus = bus;
            _configuration = configuration;
			_referenceResolver = referenceResolver;
			_preProcessors = preProcessors;
        }

        #region ITestRunner Members

        public bool CanHandleTestFor(ProjectDocument document)
        {
            return document.ContainsNUnitTests;
        }
		
		public bool CanHandleTestFor(ChangedFile assembly)
		{
			var references = _referenceResolver.GetReferences(assembly.FullName);
			return references.Contains("nunit.framework");
		}

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
			var results = new List<TestRunResults>();
			runInfos = preProcessTestRun(runInfos);
			// Get a list of the various nunit executables specified pr. framework version
			var nUnitExes = getNUnitExes(runInfos);
			foreach (var nUnitExe in nUnitExes)
			{
				// Get the assemblies that should be run under this nunit executable
				var assemblies = getAssembliesAndTestsForTestRunner(nUnitExe, runInfos);
				var arguments = getExecutableArguments(assemblies, runInfos);
				_bus.Publish(new InformationMessage(string.Format("Running tests: {0} {1}", nUnitExe, arguments)));
				Console.WriteLine("Running tests: {0} {1}", nUnitExe, arguments); 
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
		
		private TestRunInfo[] preProcessTestRun(TestRunInfo[] runInfos)
		{
			var runDetails = getRunDetails(runInfos);
			foreach (var preProcessor in _preProcessors)
				preProcessor.PreProcess(runDetails);
			return applyRunDetails(runInfos, runDetails);
		}
		
		private TestRunDetails[] getRunDetails(TestRunInfo[] runInfos)
		{
			var runDetailsList = new List<TestRunDetails>();
			foreach (var runInfo in runInfos)
				runDetailsList.Add(new TestRunDetails(TestRunnerType.NUnit, runInfo.Assembly));
			return runDetailsList.ToArray();
		}
		
		private TestRunInfo[] applyRunDetails(TestRunInfo[] runInfos, TestRunDetails[] runDetails)
		{
			foreach (var runDetail in runDetails)
			{
				var info = runInfos.Where<TestRunInfo>(i => i.Assembly.Equals(runDetail.Assembly)).First();
				info.AddTestsToRun(runDetail.TestsToRun);
			}
			return runInfos;
		}
		
		private string[] getNUnitExes(TestRunInfo[] runInfos)
		{
			var testRunnerExes = new List<string>();
			foreach (var runInfo in runInfos)
			{
				var unitTestExe = _configuration.NunitTestRunner(getFramework(runInfo));
	            if (File.Exists(unitTestExe))
				{
					if (!testRunnerExes.Exists(x => x.Equals(unitTestExe)))
	                	testRunnerExes.Add(unitTestExe);
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
					assemblies += string.Format("\"{0}\"", runInfo.Assembly) + " ";
					var assemblyTests = getTestsList(runInfo);
					if (assemblyTests.Length > 0)
						tests += (tests.Length > 0 ? "," : "") + assemblyTests;
				}
			}
			if (tests.Length > 0)
				tests = string.Format("{0}run={1}", separator, tests);
			return string.Format("{0} {1}", tests, assemblies);
		}
        
        string getExecutableArguments (string assemblyName, TestRunInfo[] runInfos)
		{
			var separator = getArgumentSeparator();
			var categoryList = getCategoryIgnoreList();
			return string.Format("{0}noshadow {0}xmlconsole {1}", separator, categoryList) + assemblyName;
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
