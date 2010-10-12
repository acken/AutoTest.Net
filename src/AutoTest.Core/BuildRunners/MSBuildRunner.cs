using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;
using System.Collections.Generic;

namespace AutoTest.Core.BuildRunners
{
    public class MSBuildRunner : IBuildRunner
    {
        private string _buildExecutable;

        public MSBuildRunner()
        {
        }

        public BuildRunResults RunBuild(string projectName, string buildExecutable)
        {
            var timer = Stopwatch.StartNew();
            _buildExecutable = buildExecutable;
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(_buildExecutable, string.Format("\"{0}\"", projectName));
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string line;
            var buildResults = new BuildRunResults(projectName);
			var lines = new List<string>();
            while ((line = process.StandardOutput.ReadLine()) != null)
				lines.Add(line);
            process.WaitForExit();
            timer.Stop();
			var parser = new MSBuildOutputParser(buildResults, lines.ToArray());
            parser.Parse();
            buildResults.SetTimeSpent(timer.Elapsed);
            return buildResults;
        }
    }
}