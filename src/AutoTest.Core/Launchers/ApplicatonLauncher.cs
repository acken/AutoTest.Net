using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using AutoTest.Core.Configuration;
using System.IO;

namespace AutoTest.Core.Launchers
{
    public class ApplicatonLauncher
    {
        private IConfiguration _configuration;

        public ApplicatonLauncher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void LaunchEditor(string file, int lineNumber, int column)
        {
            if (isMonoDevelop())
				LaunchMonoDevelop(file, lineNumber, column);
			else
				LaunchExecutable(file, lineNumber, column);
        }
		
		private bool isMonoDevelop()
		{
			var executable = _configuration.CodeEditor.Executable.ToLower();
			return executable.EndsWith("monodevelop") || executable.EndsWith("monodevelop.exe");
		}
		
		private void LaunchMonoDevelop(string file, int lineNumber, int column)
		{
			var launcher = new MonoDevelopLauncher();
			if (!launcher.Launch(file, lineNumber, column))
				LaunchExecutable(file, lineNumber, column);
		}
		
		private void LaunchExecutable(string file, int lineNumber, int column)
		{
			if (invalidLauncher())
			    return;
			var executable = _configuration.CodeEditor.Executable;
            var arguments = _configuration.CodeEditor.Arguments;
            arguments = arguments.Replace("[[CodeFile]]", file);
            arguments = arguments.Replace("[[LineNumber]]", lineNumber.ToString());
			AutoTest.Core.DebugLog.Debug.LaunchingEditor(executable, arguments);
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(executable, arguments);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
		}
		
		private bool invalidLauncher()
		{
			var invalid = !File.Exists(_configuration.CodeEditor.Executable);
			if (invalid)
				DebugLog.Debug.WriteError("Invalid launcher: {0}", _configuration.CodeEditor.Executable);
			return invalid;
		}
    }
}
