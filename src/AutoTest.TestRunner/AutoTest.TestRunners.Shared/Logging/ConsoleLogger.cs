using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.Logging
{
    public class ConsoleLogger : ILogger
    {
		private bool _outputEnabled;
		private bool _debugEnabled;

		public ConsoleLogger(bool outputEnabled, bool debugEnabled) {
			_outputEnabled = outputEnabled;
			_debugEnabled = debugEnabled;
		}

		public void Write(string message) {
			if (!_outputEnabled) return;
            Console.WriteLine(message);
        }

        public void Write(string message, params object[] args) {
			if (!_outputEnabled) return;
            Console.WriteLine(message, args);
        }

		public void WriteChunk(string message) {
			if (!_outputEnabled) return;
			Console.Write(message);
		}
        
		public void WriteChunk(string message, params object[] args) {
			if (!_outputEnabled) return;
			Console.Write(message, args);
		}

        public void Debug(string message) {
			if (!_debugEnabled) return;
            Console.WriteLine(message);
        }

        public void Debug(string message, params object[] args) {
			if (!_debugEnabled) return;
            Console.WriteLine(message, args);
        }

		public void DebugChunk(string message) {
			if (!_debugEnabled) return;
			Console.Write(message);
		}
        
		public void DebugChunk(string message, params object[] args) {
			if (!_debugEnabled) return;
			Console.Write(message, args);
		}

        public void Debug(Exception ex) {
			if (!_debugEnabled) return;
            Debug(getExceptionInfo(ex));
        }

        private string getExceptionInfo(Exception ex) {
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
                sb.AppendLine(getExceptionInfo(ex.InnerException));
            return sb.ToString();
        }
    }
}
