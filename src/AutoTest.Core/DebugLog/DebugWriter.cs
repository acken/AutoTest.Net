using System;
using System.IO;
using AutoTest.Core.FileSystem;
namespace AutoTest.Core.DebugLog
{
	public class DebugWriter : IWriteDebugInfo
	{
		private static object _padLock = new object();
        private static string _logFile = "debug.log";
		
		public void Write(string text)
		{
			lock (_padLock)
            {
				var file = Path.Combine(PathParsing.GetRootDirectory(), _logFile);
                using (var writer = getWriter(file))
                {
                    writer.WriteLine(text);
                }
                if ((new FileInfo(file)).Length > 1024000)
                {
                    File.Delete(string.Format("{0}.old", file));
                    File.Move(file, string.Format("{0}.old", file));
                }
            }
		}
		
		private static StreamWriter getWriter(string file)
		{
			if (File.Exists(file))
				return new StreamWriter(file, true);
			else
				return new StreamWriter(file);
		}
	}
}

