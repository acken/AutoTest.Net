using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching.Projects;

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

        public BuildRunResults RunBuild(Project project, string buildExecutable)
        {
            var timer = Stopwatch.StartNew();
            _buildExecutable = buildExecutable;
			var properties = buildProperties(project);
            var arguments = string.Format("\"{0}\"", project.Key) + properties;
			if (project.Value.RequiresRebuild)
				arguments += " /target:rebuild";
            DebugLog.Debug.WriteMessage(string.Format("Running build: {0} {1}", _buildExecutable, arguments));
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(_buildExecutable, arguments);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string line;
            var buildResults = new BuildRunResults(project.Key);
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
		
		private string buildProperties(Project project)
		{
			var outputDir = getOutputDir(project);
			string overriddenPlatform = "";
			// Only override platform for winodws. It's flawed on other platforms
			if (Environment.OSVersion.Platform != PlatformID.MacOSX && Environment.OSVersion.Platform != PlatformID.Unix)
			{
				if (project.Value.Platform == null || project.Value.Platform.Length.Equals(0))
					overriddenPlatform = ",Platform=AnyCPU";
			}
			return " /property:OutDir=" + outputDir + overriddenPlatform;
		}

        private string getOutputDir(Project project)
		{
            var outputPath = string.Format("bin{0}AutoTest.NET{0}", Path.DirectorySeparatorChar);
			if (_configuration.CustomOutputPath != null && _configuration.CustomOutputPath.Length > 0)
                outputPath = _configuration.CustomOutputPath;
            if (!Path.IsPathRooted(outputPath))
                outputPath = Path.Combine(Path.GetDirectoryName(project.Key), outputPath);
            if (outputPath.Substring(outputPath.Length - 1, 1) != Path.DirectorySeparatorChar.ToString())
                outputPath += Path.DirectorySeparatorChar;
			return outputPath;
		}
	}
}