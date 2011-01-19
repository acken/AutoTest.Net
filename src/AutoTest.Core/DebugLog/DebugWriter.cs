using System;
using System.IO;
using AutoTest.Core.FileSystem;
namespace AutoTest.Core.DebugLog
{
	public class DebugWriter : IWriteDebugInfo
	{
		private object _padLock = new object();
        private string _logFile = "debug.log";
		private string _path;
		
		public DebugWriter(string path)
		{
			_path = path;
		}
		
		public DebugWriter(string path, string filename)
		{
			_path = path;
			_logFile = filename;
		}

        public void WriteError(string message)
        {
            write(message);
        }

        public void WriteDebug(string message)
        {
            write(message);
        }

        public void WriteInfo(string message)
        {
            write(message);
        }

        public void WritePreProcessor(string message)
        {
            write(message);
        }

        public void WriteDetail(string message)
        {
            write(message);
        }
		
		private void write(string text)
		{
			lock (_padLock)
            {
				var file = Path.Combine(_path, _logFile);
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
		
		private StreamWriter getWriter(string file)
		{
			if (File.Exists(file))
				return new StreamWriter(file, true);
			else
				return new StreamWriter(file);
		}
    }
}

