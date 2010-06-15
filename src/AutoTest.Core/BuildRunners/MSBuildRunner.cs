namespace AutoTest.Core.BuildRunners
{
    using System;
    using System.Diagnostics;
    using log4net;
    using System.Text;
    using System.IO;

    public class MSBuildRunner : IBuildRunner
    {
        private readonly string _buildExecutable;
        private BuildRunResults _buildResults;
        static readonly ILog _logger = LogManager.GetLogger(typeof(MSBuildRunner));

        public MSBuildRunner(string buildExecutable)
        {
            _buildExecutable = buildExecutable;
        }

        public BuildRunResults RunBuild(string projectName)
        {
            _logger.InfoFormat("Starting build of {0} using \"{1}\".",
                Path.GetFileName(projectName),
                Path.GetFileName(_buildExecutable));
            _buildResults = new BuildRunResults();
            Process process = new Process();
            process.OutputDataReceived += process_OutputDataReceived;
            process.StartInfo = new ProcessStartInfo(_buildExecutable, string.Format("\"{0}\"", projectName));
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            process.WaitForExit();
            return _buildResults;
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string line = e.Data.Trim();
            var parser = new MSBuildOutputParser(_buildResults, line);
            parser.Parse();
        }
    }
}