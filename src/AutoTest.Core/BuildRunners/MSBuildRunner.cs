using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using Castle.Core.Logging;

namespace AutoTest.Core.BuildRunners
{
    public class MSBuildRunner : IBuildRunner
    {
        private readonly string _buildExecutable;
        private ILogger _logger;

        public ILogger Logger
        {
            get { if (_logger == null) _logger = NullLogger.Instance; return _logger; }
            set { _logger = value; }
        }

        public MSBuildRunner(string buildExecutable)
        {
            _buildExecutable = buildExecutable;
        }

        public BuildRunResults RunBuild(string projectName)
        {
            Logger.InfoFormat("Starting build of {0} using \"{1}\".",
                Path.GetFileName(projectName),
                Path.GetFileName(_buildExecutable));
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(_buildExecutable, string.Format("\"{0}\"", projectName));
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string line;
            var buildResults = new BuildRunResults();
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