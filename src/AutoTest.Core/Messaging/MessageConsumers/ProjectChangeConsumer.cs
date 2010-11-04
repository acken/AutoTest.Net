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
using AutoTest.Messages;

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
        private IPreProcessTestruns[] _preProcessors;

        public ProjectChangeConsumer(IMessageBus bus, IGenerateBuildList listGenerator, IConfiguration configuration, IBuildRunner buildRunner, ITestRunner[] testRunners, IDetermineIfAssemblyShouldBeTested testAssemblyValidator, IOptimizeBuildConfiguration buildOptimizer, IPreProcessTestruns[] preProcessors)
        {
            _bus = bus;
            _listGenerator = listGenerator;
            _configuration = configuration;
            _buildRunner = buildRunner;
            _testRunners = testRunners;
			_testAssemblyValidator = testAssemblyValidator;
			_buildOptimizer = buildOptimizer;
            _preProcessors = preProcessors;
        }

        #region IConsumerOf<ProjectChangeMessage> Members

        public void Consume(ProjectChangeMessage message)
        {
            Debug.ConsumingProjectChangeMessage(message);
            _bus.Publish(new RunStartedMessage(message.Files));
            var runReport = execute(message);
            _bus.Publish(new RunFinishedMessage(runReport));
            informPreProcessor(runReport);
        }

        private void informPreProcessor(RunReport runReport)
        {
            foreach (var preProcess in _preProcessors)
                preProcess.RunFinished(runReport);
        }

        private RunReport execute(ProjectChangeMessage message)
        {
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
            projectList = preProcessTestRun(projectList);
            foreach (var runner in _testRunners)
            {
				var runInfos = new List<TestRunInfo>();
				foreach (var file in projectList)
	            {
					var project = file.Project;
					if (hasInvalidOutputPath(project))
						continue;
					string folder = Path.Combine(Path.GetDirectoryName(project.Key), project.Value.OutputPath);
					
					if (hasInvalidAssemblyName(project))
						continue;
	            	var assembly = Path.Combine(folder, project.Value.AssemblyName);
					
					if (_testAssemblyValidator.ShouldNotTestAssembly(assembly))
					    continue;
					if (!project.Value.ContainsTests)
	                	continue;
                    if (runner.CanHandleTestFor(project.Value))
                    {
                        var runInfo = new TestRunInfo(project, assembly);
                        runInfo.AddTestsToRun(file.TestsToRun);
                        if (file.OnlyRunSpcifiedTests)
                            runInfo.ShouldOnlyRunSpcifiedTests();
                        runInfos.Add(runInfo);
                    }
					_bus.Publish(new RunInformationMessage(InformationType.TestRun, project.Key, assembly, runner.GetType()));
				}
				if (runInfos.Count > 0)
					runTests(runner, runInfos.ToArray(), runReport);
			}
		}

        private RunInfo[] preProcessTestRun(RunInfo[] runInfos)
        {
            foreach (var preProcessor in _preProcessors)
                preProcessor.PreProcess(runInfos);
            return runInfos;
        }
		
		private bool hasInvalidOutputPath(Project project)
		{
			if (project.Value.OutputPath == null)
			{
				_bus.Publish(new ErrorMessage(string.Format("Output path was unexpectedly set to null for {0}. Skipping assembly", project.Key)));
				return true;
			}
			return false;
		}
		
		private bool hasInvalidAssemblyName(Project project)
		{
			if (project.Value.AssemblyName == null)
			{
				_bus.Publish(new ErrorMessage(string.Format("Assembly name was unexpectedly set to null for {0}. Skipping assembly", project.Key)));
				return true;
			}
			return false;
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
