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
using System.Reflection;
using AutoTest.Core.FileSystem;
using AutoTest.Messages;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class MSTestRunner : ITestRunner
    {
        private IConfiguration _configuration;
		private IResolveAssemblyReferences _referenceResolver;

        public MSTestRunner(IConfiguration configuration, IResolveAssemblyReferences referenceResolver)
        {
            _configuration = configuration;
			_referenceResolver = referenceResolver;
        }

        #region ITestRunner Members

        public bool CanHandleTestFor(ProjectDocument document)
        {
            return document.ContainsMSTests;
        }
		
		public bool CanHandleTestFor(string assembly)
		{
			var references = _referenceResolver.GetReferences(assembly);
			return references.Contains("Microsoft.VisualStudio.QualityTools.UnitTestFramework");
		}

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
			var results = new List<TestRunResults>();
			foreach (var runInfo in runInfos)
			{			
	            var timer = Stopwatch.StartNew();
	            var unitTestExe = _configuration.MSTestRunner(getFramework(runInfo));
	            if (!File.Exists(unitTestExe))
				{
					var project = "";
					if (runInfo.Project != null)
						project = runInfo.Project.Key;
	                results.Add(new TestRunResults(project, runInfo.Assembly, false, new TestResult[] { }));
					continue;
				}
				
                if (runInfo.OnlyRunSpcifiedTestsFor(TestRunner.MSTest) && runInfo.GetTestsFor(TestRunner.MSTest).Length.Equals(0))
                    continue;
				var calc = new MaxCmdLengthCalculator();
				var tests = getTestsList(runInfo);
				var arguments = "/testcontainer:\"" + runInfo.Assembly + "\" " + tests + " /detail:errorstacktrace /detail:errormessage";
                var shouldContainTests = (arguments.Length + unitTestExe.Length) > calc.GetLength();
				if (shouldContainTests)
					arguments = "/testcontainer:\"" + runInfo.Assembly + "\"" + " /detail:errorstacktrace /detail:errormessage";
				DebugLog.Debug.WriteMessage(string.Format("Running tests: {0} {1}", unitTestExe, arguments)); 
	            var proc = new Process();
	            proc.StartInfo = new ProcessStartInfo(unitTestExe, arguments);
	            proc.StartInfo.RedirectStandardOutput = true;
	            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(runInfo.Assembly);
	            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
	            proc.StartInfo.UseShellExecute = false;
	            proc.StartInfo.CreateNoWindow = true;
	
	            proc.Start();
	            string line;
	            var parser = new MSTestResponseParser(runInfo.Project.Key, runInfo.Assembly, shouldContainTests);
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
		
		private string getTestsList(TestRunInfo runInfo)
		{
			var tests = "";
			foreach (var test in runInfo.GetTestsFor(TestRunner.MSTest))
				tests += string.Format("/test:{0} ", test);
			return tests;
		}
		
		private string getFramework(TestRunInfo runInfo)
		{
			if (runInfo.Project == null)
				return "";
			return runInfo.Project.Value.Framework;
		}
    }
}
