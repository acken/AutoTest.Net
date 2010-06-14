namespace AutoTest.Core.BuildRunners
{
    using System;
    using System.Diagnostics;
    using log4net;
    using System.Text;
    using System.IO;

    public class MSBuildRunner : IBuildRunner
    {
        readonly string _buildExecutable;
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
            ProcessStartInfo psi = new ProcessStartInfo(_buildExecutable,
                                                        string.Format("\"{0}\"", projectName));
            psi.RedirectStandardOutput = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process proc = Process.Start(psi);

            proc.WaitForExit();

            int errors = 0;
            int warnings = 0;

            var builder = new StringBuilder();
            while(!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine().Trim();
                builder.AppendLine(line);

                if(line.EndsWith("Error(s)"))
                {
                    errors = Int32.Parse(line.Substring(0, line.IndexOf(" ")));
                }
                else if(line.EndsWith("Warning(s)"))
                {
                    warnings = Int32.Parse(line.Substring(0, line.IndexOf(" ")));
                }
            }

            return new BuildRunResults(errors, warnings, builder.ToString());
        }
    }
}