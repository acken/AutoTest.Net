using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.Core.Configuration;

namespace AutoTest.Core.BuildRunners
{
    public class MSBuildRunner : IBuildRunner
    {
        private string _buildExecutable;
		private IConfiguration _configuration;

        public MSBuildRunner(IConfiguration configuration)
        {
			_configuration = configuration;
        }

        public BuildRunResults RunBuild(string projectName, string buildExecutable)
        {
            var timer = Stopwatch.StartNew();
            _buildExecutable = buildExecutable;
			var outputDir = getOutputDir();
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(_buildExecutable, string.Format("\"{0}\"", projectName) + " /property:OutDir=" + outputDir);
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
        
		private string getOutputDir()
		{
			if (_configuration.CustomOutputPath != null && _configuration.CustomOutputPath.Length > 0)
			{
				var outputPath = _configuration.CustomOutputPath;
				if (outputPath.Substring(outputPath.Length - 1, 1) != Path.DirectorySeparatorChar.ToString())
					outputPath += Path.DirectorySeparatorChar;
				return outputPath;
			}
			return "bin/AutoTest.NET/";
		}
	}
}