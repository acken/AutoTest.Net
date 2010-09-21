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
        private ICache _cache;
        private IConfiguration _configuration;
        private IBuildRunner _buildRunner;
        private ITestRunner[] _testRunners;

        public ProjectChangeConsumer(IMessageBus bus, IGenerateBuildList listGenerator, ICache cache, IConfiguration configuration, IBuildRunner buildRunner, ITestRunner[] testRunners)
        {
            _bus = bus;
            _listGenerator = listGenerator;
            _cache = cache;
            _configuration = configuration;
            _buildRunner = buildRunner;
            _testRunners = testRunners;
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
            var list = _listGenerator.Generate(getListOfChangedProjects(message));
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
		
		private bool buildAll(string[] projectList, RunReport runReport)
		{
			foreach (var file in projectList)
            {
                if (!build(_cache.Get<Project>(file), runReport))
                    return false;
            }
			return true;
		}
		
		private void testAll(string[] projectList, RunReport runReport)
		{
			foreach (var file in projectList)
            {
				var project = _cache.Get<Project>(file);
				if (project.Value.ContainsTests)
                	runTests(project, runReport);
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

        private void runTests(Project project, RunReport runReport)
        {
            string folder = Path.Combine(Path.GetDirectoryName(project.Key), project.Value.OutputPath);
            var file = Path.Combine(folder, project.Value.AssemblyName);
            foreach (var runner in _testRunners)
            {
                if (runner.CanHandleTestFor(project.Value))
                    runTests(runner, project, file, runReport);
            }
        }

        #endregion

        private void runTests(ITestRunner testRunner, Project project, string assembly, RunReport runReport)
        {
            _bus.Publish(new RunInformationMessage(InformationType.TestRun, project.Key, assembly, testRunner.GetType()));
            var results = testRunner.RunTests(project, assembly);
            runReport.AddTestRun(
                results.Project,
                results.Assembly,
                results.TimeSpent,
                results.Passed.Length,
                results.Ignored.Length,
                results.Failed.Length);
            _bus.Publish(new TestRunMessage(results));
        }
    }
}
