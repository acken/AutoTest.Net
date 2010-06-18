using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;

namespace AutoTest.Core.BuildRunners
{
    public class MSBuildRunner : IBuildRunner
    {
        private readonly string _buildExecutable;
        private readonly IMessageBus _bus;

        public MSBuildRunner(string buildExecutable, IMessageBus bus)
        {
            _buildExecutable = buildExecutable;
            _bus = bus;
        }

        public BuildRunResults RunBuild(string projectName)
        {
            _bus.Publish(new InformationMessage(
                             string.Format("Starting build of {0} using \"{1}\".",
                                           Path.GetFileName(projectName),
                                           Path.GetFileName(_buildExecutable))));
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(_buildExecutable, string.Format("\"{0}\"", projectName));
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string line;
            var buildResults = new BuildRunResults(projectName);
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                var parser = new MSBuildOutputParser(buildResults, line);
                parser.Parse();
            }
            process.WaitForExit();
            return buildResults;
        }
    }
}