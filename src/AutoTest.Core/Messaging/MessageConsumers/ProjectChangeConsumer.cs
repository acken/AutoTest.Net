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

namespace AutoTest.Core.Messaging.MessageConsumers
{
    class ProjectChangeConsumer : IConsumerOf<ProjectChangeMessage>
    {
        private IMessageBus _bus;
        private ICache _cache;
        private IConfiguration _configuration;

        public ProjectChangeConsumer(IMessageBus bus, ICache cache, IConfiguration configuration)
        {
            _bus = bus;
            _cache = cache;
            _configuration = configuration;
        }

        #region IConsumerOf<ProjectChangeMessage> Members

        public void Consume(ProjectChangeMessage message)
        {
            _bus.Publish(new InformationMessage(""));
            _bus.Publish(new InformationMessage("Preparing build(s) and test run(s)"));
            var runReport = new RunReport();
            foreach (var file in message.Files)
            {
                var project = _cache.Get<Project>(file.FullName);
                // Prioritized tests that test me
                // Other prioritized tests
                // Projects that tests me
                // Other test projects
                var report = buildAndRunTests(project);
                runReport.NumberOfProjectsBuilt += report.NumberOfProjectsBuilt;
                runReport.NumberOfTestsRan += report.NumberOfTestsRan;
            }
            _bus.Publish(new InformationMessage(string.Format("Ran {0} build(s) and {1} test(s)", runReport.NumberOfProjectsBuilt, runReport.NumberOfTestsRan)));
        }

        private RunReport buildAndRunTests(Project project)
        {
            var runReport = new RunReport();
            runReport.NumberOfProjectsBuilt = 1;
            if (!buildProject(project.Key))
                return runReport;
            if (project.Value.ContainsTests)
                runReport.NumberOfTestsRan += runTests(project.Key);
            foreach (var reference in project.Value.ReferencedBy)
            {
                var referencerunReport = buildAndRunTests(_cache.Get<Project>(reference));
                runReport.NumberOfProjectsBuilt += referencerunReport.NumberOfProjectsBuilt;
                runReport.NumberOfTestsRan += referencerunReport.NumberOfTestsRan;
            }
            return runReport;
        }

        private bool buildProject(string project)
        {
            var buildRunner = new MSBuildRunner(_configuration.BuildExecutable);
            var buildReport = buildRunner.RunBuild(project);
            if (buildReport.ErrorCount > 0 || buildReport.WarningCount > 0)
            {
                if (buildReport.ErrorCount > 0)
                {
                    _bus.Publish(new InformationMessage(string.Format(
                        "Building {0} finished with {1} errors and  {2} warningns",
                        Path.GetFileName(project),
                        buildReport.ErrorCount,
                        buildReport.WarningCount)));
                }
                else
                {
                    _bus.Publish(new InformationMessage(string.Format(
                        "Building {0} succeeded with {1} warnings",
                        Path.GetFileName(project),
                        buildReport.WarningCount)));
                }
                foreach (var error in buildReport.Errors)
                    _bus.Publish(new InformationMessage(string.Format("Error: {0}({1},{2}) {3}", error.File, error.LineNumber, error.LinePosition,
                                      error.ErrorMessage)));
                foreach (var warning in buildReport.Warnings)
                    _bus.Publish(new InformationMessage(string.Format("Warning: {0}({1},{2}) {3}", warning.File, warning.LineNumber,
                                      warning.LinePosition, warning.ErrorMessage)));
            }
            return buildReport.ErrorCount == 0;
        }

        private int runTests(string projectPath)
        {
            int numberOfTests = 0;
            var project = _cache.Get<Project>(projectPath);
            string folder = Path.Combine(Path.GetDirectoryName(projectPath), project.Value.OutputPath);

            var file = Path.Combine(folder, project.Value.AssemblyName);
            if (project.Value.ContainsNUnitTests)
                numberOfTests += runTests(new NUnitTestRunner(_configuration), file);
            if (project.Value.ContainsMSTests)
                numberOfTests += runTests(new MSTestRunner(_configuration), file);
            return numberOfTests;
        }

        #endregion

        private int runTests(ITestRunner testRunner, string assembly)
        {
            var results = testRunner.RunTests(assembly);
            var failed = results.Failed;
            var ignored = results.Ignored;
            if (failed.Length > 0 || ignored.Length > 0)
            {
                _bus.Publish(new InformationMessage(string.Format("Test(s) {0} for assembly {1}", failed.Length > 0 ? "failed" : "was ignored", Path.GetFileName(assembly))));
                foreach (var result in results.Failed)
                    _bus.Publish(new InformationMessage(string.Format("    {0} -> {1}", result.Status, result.Message)));
                foreach (var result in results.Ignored)
                    _bus.Publish(new InformationMessage(string.Format("    {0} -> {1}", result.Status, result.Message)));
            }
            return results.All.Length;
        }
    }
}
