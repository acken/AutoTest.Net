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
using AutoTest.Core.FileSystem;
using AutoTest.Messages;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class XUnitTestRunner : ITestRunner
    {
        private IMessageBus _bus;
        private IConfiguration _configuration;
		private IResolveAssemblyReferences _referenceResolver;
        private IFileSystemService _fsService;

        public XUnitTestRunner(IMessageBus bus, IConfiguration configuration, IResolveAssemblyReferences referenceResolver, IFileSystemService fsService)
        {
            _bus = bus;
            _configuration = configuration;
			_referenceResolver = referenceResolver;
            _fsService = fsService;
        }

        #region ITestRunner Members

        public bool CanHandleTestFor(Project project)
        {
            return project.Value.ContainsXUnitTests && _fsService.FileExists(_configuration.XunitTestRunner(project.Value.Framework));
        }

        public bool CanHandleTestFor(string assembly)
		{
			var references = _referenceResolver.GetReferences(assembly);
			return references.Contains("xunit");
		}

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
			var results = new List<TestRunResults>();
			foreach (var runInfo in runInfos)
			{
	            var unitTestExe = _configuration.XunitTestRunner(getFramework(runInfo));
	            if (!File.Exists(unitTestExe))
				{
					var project = "";
					if (runInfo.Project != null)
						project = runInfo.Project.Key;
	                results.Add(new TestRunResults(project, runInfo.Assembly, false, TestRunner.XUnit, new TestResult[] { }));
					continue;
				}
	
	            var resultFile = Path.GetTempFileName();
	            var arguments = string.Format("\"{0}\" /noshadow /nunit \"{1}\"", runInfo.Assembly, resultFile);
				DebugLog.Debug.WriteInfo("Running tests: {0} {1}", unitTestExe, arguments); 
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
	            var parser = new NUnitTestResponseParser(_bus, TestRunner.XUnit);
	            using (TextReader reader = new StreamReader(resultFile))
	                parser.Parse(reader.ReadToEnd(), runInfos, false);
	            File.Delete(resultFile);
				foreach (var result in parser.Result)
		            results.Add(result);
			}
			return results.ToArray();
        }

        #endregion
		
		private string getFramework(TestRunInfo runInfo)
		{
			if (runInfo.Project == null)
				return "";
			return runInfo.Project.Value.Framework;
		}
    }
}
