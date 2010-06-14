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

namespace AutoTest.Core.FileSystem
{
    class BuildLocator : IConsumerOf<ProjectChangeMessage>
    {
        private ICache _cache;
        private IConfiguration _configuration;

        public BuildLocator(ICache cache, IConfiguration configuration)
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
            Console.WriteLine(string.Format("Build finished with {0} errors and  {1} warningns", buildReport.Errors, buildReport.Warnings));
            if (buildReport.Errors > 0)
                Console.WriteLine(buildReport.BuildOutput);
            return buildReport.Errors == 0;
        }

        private void runTests(string project)
        {
            string folder = Path.Combine(Path.GetDirectoryName(project), string.Format("bin{0}Debug", Path.DirectorySeparatorChar));
            var files = Directory.GetFiles(folder, string.Format("{0}.*", Path.GetFileNameWithoutExtension(project)));
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                if (extension == ".dll" || extension == ".exe")
                {
                    var testRunner = new CommandLineTestRunner(_configuration);
                    var results = testRunner.RunTests(file);
                    foreach (var result in results.All)
                    {
                        Console.WriteLine(string.Format("{0} {1}", result.Status, result.Message));
                    }
                }
            }
            
        }

        #endregion
    }
}
