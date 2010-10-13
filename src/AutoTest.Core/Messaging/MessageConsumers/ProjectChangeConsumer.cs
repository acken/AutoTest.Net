using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.TestRunners;
using System.IO;
using AutoTest.Core.TestRunners.TestRunners;
using Castle.Core.Logging;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    class ProjectChangeConsumer : IBlockingConsumerOf<ProjectChangeMessage>
    {
        private IMessageBus _bus;
        private IGenerateBuildList _listGenerator;
        private IConfiguration _configuration;
        private IBuildRunner _buildRunner;
        private ITestRunner[] _testRunners;
		private IDetermineIfAssemblyShouldBeTested _testAssemblyValidator;
		private IOptimizeBuildConfiguration _buildOptimizer;

        public ProjectChangeConsumer(IMessageBus bus, IGenerateBuildList listGenerator, IConfiguration configuration, IBuildRunner buildRunner, ITestRunner[] testRunners, IDetermineIfAssemblyShouldBeTested testAssemblyValidator, IOptimizeBuildConfiguration buildOptimizer)
        {
            _bus = bus;
            _listGenerator = listGenerator;
            _configuration = configuration;
            _buildRunner = buildRunner;
            _testRunners = testRunners;
			_testAssemblyValidator = testAssemblyValidator;
			_buildOptimizer = buildOptimizer;
        }

        #region IConsumerOf<ProjectChangeMessage> Members

        public void Consume(ProjectChangeMessage message)
        {
            Debug.ConsumingProjectChangeMessage(message);
            _bus.Publish(new RunStartedMessage(message.Files));
            var runReport = execute(message);
            _bus.Publish(new RunFinishedMessage(runReport));
        }

        private RunReport execute(ProjectChangeMessage message)
        {
            // Prioritized tests that test me
            // Other prioritized tests
            // Projects that tests me
            // Other test projects
            var runReport = new RunReport();
            var projectsAndDependencies = _listGenerator.Generate(getListOfChangedProjects(message));
			var list = _buildOptimizer.AssembleBuildConfiguration(projectsAndDependencies);
            if (!buildAll(list, runReport))
				return runReport;
			testAll(list, runReport);
            return runReport;
        }

        private string[] getListOfChangedProjects(ProjectChangeMessage message)
        {
            var projects = new List<string>();
            foreach (var file in message.Files)
                projects.Add(file.FullName);
            return projects.ToArray();
        }
		
		private bool buildAll(RunInfo[] projectList, RunReport runReport)
		{
			var indirectlyBuild = new List<string>();
			foreach (var file in projectList)
            {
				if (file.ShouldBeBuilt)
				{
					Debug.WriteMessage(string.Format("Set to build project {0}", file.Project.Key));
	                if (!build(file.Project, runReport))
	                    return false;
				}
				else
				{
					Debug.WriteMessage(string.Format("Not set to build project {0}", file.Project.Key));
					indirectlyBuild.Add(file.Project.Key);
				}
            }
			foreach (var project in indirectlyBuild)
				runReport.AddBuild(project, new TimeSpan(0), true);
			return true;
		}
		
		private void testAll(RunInfo[] projectList, RunReport runReport)
		{
            foreach (var runner in _testRunners)
            {
				var runInfos = new List<TestRunInfo>();
				foreach (var file in projectList)
	            {
					var project = file.Project;
					string folder = Path.Combine(Path.GetDirectoryName(project.Key), project.Value.OutputPath);
	            	var assembly = Path.Combine(folder, project.Value.AssemblyName);
					if (_testAssemblyValidator.ShouldNotTestAssembly(assembly))
					    continue;
					if (!project.Value.ContainsTests)
	                	continue;
					if (runner.CanHandleTestFor(project.Value))
                    	runInfos.Add(new TestRunInfo(project, assembly));
					_bus.Publish(new RunInformationMessage(InformationType.TestRun, project.Key, assembly, runner.GetType()));
				}
				if (runInfos.Count > 0)
					runTests(runner, runInfos.ToArray(), runReport);
			}
		}

        private bool build(Project project, RunReport runReport)
        {
            if (File.Exists(_configuration.BuildExecutable(project.Value)))
            {
                _bus.Publish(new RunInformationMessage(
                                 InformationType.Build,
                                 project.Key,
                                 project.Value.AssemblyName,
                                 typeof(MSBuildRunner)));
                if (!buildProject(project, runReport))
                    return false;
            }

            return true;
        }

        private bool buildProject(Project project, RunReport report)
        {
            var buildReport = _buildRunner.RunBuild(project.Key, _configuration.BuildExecutable(project.Value));
            var succeeded = buildReport.ErrorCount == 0;
            report.AddBuild(buildReport.Project, buildReport.TimeSpent, succeeded);
            _bus.Publish(new BuildRunMessage(buildReport));
            return succeeded;
        }

        #endregion

        private void runTests(ITestRunner testRunner, TestRunInfo[] runInfos, RunReport runReport)
        {
            var results = testRunner.RunTests(runInfos);
			foreach (var result in results)
			{
	            runReport.AddTestRun(
	                result.Project,
	                result.Assembly,
	                result.TimeSpent,
	                result.Passed.Length,
	                result.Ignored.Length,
	                result.Failed.Length);
	            _bus.Publish(new TestRunMessage(result));
			}
        }
    }
}
