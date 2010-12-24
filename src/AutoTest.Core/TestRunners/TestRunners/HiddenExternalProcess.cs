using System.Diagnostics;
using System.IO;

namespace AutoTest.Core.TestRunners.TestRunners
{
    public interface IExternalProcess
    {
        void CreateAndWaitForExit(string executable, string arguments);
    }

    internal class HiddenExternalProcess : IExternalProcess
    {
        public void CreateAndWaitForExit(string executable, string arguments)
        {
            var process = new Process
                          {
                              StartInfo = new ProcessStartInfo(executable, arguments)
                                          {
                                              RedirectStandardOutput = true,
                                              WorkingDirectory = Path.GetDirectoryName(executable),
                                              WindowStyle = ProcessWindowStyle.Hidden,
                                              UseShellExecute = false,
                                              CreateNoWindow = true
                                          }
                          };
            DebugLog.Debug.WriteMessage(string.Format("Launching: {0} {1}", executable, arguments));
            process.Start();

            WaitForExit(process);
        }

        static void WaitForExit(Process process)
        {
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
    }
}