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
		private IConfiguration _configuration;

        public MSBuildRunner(IConfiguration configuration)
        {
			_configuration = configuration;
        }

        public BuildRunResults RunBuild(string solution, bool rebuild, string buildExecutable, Func<bool> abortIfTrue)
        {
            var outdir = string.Format("bin{0}AutoTest.Net{0}", Path.DirectorySeparatorChar);
            if (_configuration.CustomOutputPath.Length > 0)
                outdir = _configuration.CustomOutputPath;
            if (outdir.Substring(outdir.Length - 1, 1) != Path.DirectorySeparatorChar.ToString())
                outdir += Path.DirectorySeparatorChar;
            var arguments = string.Format("\"{0}\" /property:OutDir={1};IntermediateOutputPath={2}", solution, outdir, Path.Combine(outdir, "_tmpIntermediateAT") + Path.DirectorySeparatorChar);
            if (rebuild)
                arguments += " /target:rebuild";
            return runBuild(buildExecutable, arguments, solution, abortIfTrue);
        }

        public BuildRunResults RunBuild(Project project, string buildExecutable, Func<bool> abortIfTrue)
        {
			var properties = buildProperties(project);
            var arguments = string.Format("\"{0}\"", project.Key) + properties;
			if (project.Value.RequiresRebuild)
				arguments += " /target:rebuild";
            var target = project.Key;
            return runBuild(buildExecutable, arguments, target, abortIfTrue);
        }

        private static BuildRunResults runBuild(string buildExecutable, string arguments, string target, Func<bool> abortIfTrue)
        {
            var timer = Stopwatch.StartNew();
            DebugLog.Debug.WriteInfo("Running build: {0} {1}", buildExecutable, arguments);
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(buildExecutable, arguments);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string line;
            var buildResults = new BuildRunResults(target);
            var lines = new List<string>();
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                if (abortIfTrue.Invoke())
                {
                    AutoTest.Core.DebugLog.Debug.WriteDebug("Aborting build run");
                    return new BuildRunResults(target);
                }
                lines.Add(line);
            }
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
            var imoutputDir = Path.Combine(outputDir, "_tmpIntermediateAT") + Path.DirectorySeparatorChar;
			return " /property:OutDir=" + outputDir + ";IntermediateOutputPath=" + imoutputDir + overriddenPlatform;
		}

        private string getOutputDir(Project project)
		{
            var outputPath = string.Format("bin{0}AutoTest.Net{0}", Path.DirectorySeparatorChar);
			if (_configuration.CustomOutputPath != null && _configuration.CustomOutputPath.Length > 0)
                outputPath = _configuration.CustomOutputPath;
            if (outputPath.Substring(outputPath.Length - 1, 1) != Path.DirectorySeparatorChar.ToString())
                outputPath += Path.DirectorySeparatorChar;
			return outputPath;
		}
	}
}