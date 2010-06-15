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

namespace AutoTest.Core.Messaging.MessageConsumers
{
    class ProjectChangeConsumer : IConsumerOf<ProjectChangeMessage>
    {
        private ICache _cache;
        private IConfiguration _configuration;

        public ProjectChangeConsumer(ICache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        #region IConsumerOf<ProjectChangeMessage> Members

        public void Consume(ProjectChangeMessage message)
        {
            foreach (var file in message.Files)
            {
                var project = _cache.Get<Project>(file.FullName);
                // Prioritized tests that test me
                // Other prioritized tests
                // Projects that tests me
                // Other test projects
                if (buildProject(project.Key))
                {
                    if (project.Value.ContainsTests)
                        runTests(project.Key);
                    foreach (var reference in project.Value.ReferencedBy)
                    {
                        buildProject(reference);
                        var referencedProject = _cache.Get<Project>(reference);
                        if (referencedProject.Value.ContainsTests)
                            runTests(referencedProject.Key);
                    }
                }
                Console.WriteLine("Finished builds and tests");
                Console.WriteLine("");
            }
        }

        private bool buildProject(string project)
        {
            var buildRunner = new MSBuildRunner(_configuration.BuildExecutable);
            var buildReport = buildRunner.RunBuild(project);
            Console.WriteLine(string.Format("Build finished with {0} errors and  {1} warningns", buildReport.ErrorCount, buildReport.WarningCount));
            foreach (var error in buildReport.Errors)
                Console.WriteLine("Error: {0}({1},{2}) {3}", error.File, error.LineNumber, error.LinePosition, error.ErrorMessage);
            foreach (var warning in buildReport.Warnings)
                Console.WriteLine("Warning: {0}({1},{2}) {3}", warning.File, warning.LineNumber, warning.LinePosition, warning.ErrorMessage);
            return buildReport.ErrorCount == 0;
        }

        private void runTests(string projectPath)
        {
            var project = _cache.Get<Project>(projectPath);
            string folder = Path.Combine(Path.GetDirectoryName(projectPath), project.Value.OutputPath);

            var file = Path.Combine(folder, project.Value.AssemblyName);
            if (project.Value.ContainsNUnitTests)
                runTests(new NUnitTestRunner(_configuration), file);
            if (project.Value.ContainsMSTests)
                runTests(new MSTestRunner(_configuration), file);
            
        }

        #endregion

        private void runTests(ITestRunner testRunner, string assembly)
        {
            Console.WriteLine(string.Format("Running tests for {0} through {1}", Path.GetFileName(assembly), testRunner.GetType().ToString()));
            var results = testRunner.RunTests(assembly);
            foreach (var result in results.All)
                Console.WriteLine(string.Format("{0} {1}", result.Status, result.Message));
        }
    }
}
