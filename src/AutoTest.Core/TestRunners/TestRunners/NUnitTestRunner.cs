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
		
		public bool CanHandleTestFor(ChangedFile assembly)
		{
			var references = _referenceResolver.GetReferences(assembly.FullName);
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
		
		private string getAssembliesFromTestRunner(string testRunnerExes, TestRunInfo[] runInfos)
		{
			var assemblies = "";
			foreach (var runInfo in runInfos)
			{
				var unitTestExe = _configuration.NunitTestRunner(getFramework(runInfo));
				if (unitTestExe.Equals(testRunnerExes))
					assemblies += string.Format("\"{0}\"", runInfo.Assembly) + " ";
			}
			return assemblies;
		}
        
        string getExecutableArguments (string assemblyName)
		{
			var separator = getArgumentSeparator();
			var categoryList = getCategoryIgnoreList();
			return string.Format("{0}noshadow {0}xmlconsole {1}", separator, categoryList) + assemblyName;
		}

        #endregion
		
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
