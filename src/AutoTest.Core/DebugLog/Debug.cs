using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;

namespace AutoTest.Core.DebugLog
{
    public static class Debug
    {
        private static bool _isDisabled = true;
        private static object _padLock = new object();
        private static string _logFile = "debug.log";

        private static void write(string text)
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

        public static void EnableLogging()
        {
            _isDisabled = false;
        }

        public static void DisableLogging()
        {
            _isDisabled = true;
        }

        public static void InitialConfigurationFinished()
        {
            if (_isDisabled) return;
            write("Initial configuration finished");
        }

        public static void InitializedCache()
        {
            if (_isDisabled) return;
            write("Cache initialized");
        }

        public static void RegisteredAssembly(Assembly assembly)
        {
            if (_isDisabled) return;
            write(string.Format("Registered assembly {0}", assembly.FullName));
        }

        public static void ShutingDownContainer()
        {
            if (_isDisabled) return;
            write("Shuting down configuration");
        }

        public static void RawFileChangeDetected(string file)
        {
            if (_isDisabled) return;
            write(string.Format("Directory watcher found a change in file: {0}", file));
        }

        public static void AboutToPublishFileChanges(int numberOfFiles)
        {
            if (_isDisabled) return;
            write(string.Format("Directory watcher about to publish change for {0} files", numberOfFiles));
        }

        internal static void Publishing<T>()
        {
            if (_isDisabled) return;
            write(string.Format("Publishing message of type {0}", typeof(T)));
        }

        internal static void WitholdingMessage(object message)
        {
            if (_isDisabled) return;
            write(string.Format("Message bus witheld a message of type {0}", message.GetType()));
        }

        internal static void Blocking<T>()
        {
            if (_isDisabled) return;
            write(string.Format("Message bus started blocking {0}", typeof(T)));
        }

        internal static void ConsumingFileChange(FileChangeMessage message)
        {
            if (_isDisabled) return;
            var builder = new StringBuilder();
            builder.AppendLine("Consuming file change for:");
            foreach (var file in message.Files)
                builder.AppendLine(string.Format("    {0}", file.FullName));
            write(builder.ToString());
        }

        internal static void AboutToPublishProjectChanges(ProjectChangeMessage projectChange)
        {
            if (_isDisabled) return;
            write(string.Format("File change consumer about to publish change for {0} files", projectChange.Files.Length));
        }

        internal static void ConsumingProjectChangeMessage(ProjectChangeMessage message)
        {
            if (_isDisabled) return;
            var builder = new StringBuilder();
            builder.AppendLine("Consuming project changes for:");
            foreach (var file in message.Files)
                builder.AppendLine(string.Format("    {0}", file.FullName));
            write(builder.ToString());
        }

        internal static void PresenterRecievedRunStartedMessage()
        {
            if (_isDisabled) return;
            write("Presenter received run start message");
        }

        internal static void PresenterRecievedRunFinishedMessage()
        {
            if (_isDisabled) return;
            write("Presenter received run finished message");
        }

        internal static void PresenterRecievedBuildMessage()
        {
            if (_isDisabled) return;
            write("Presenter received build message");
        }

        internal static void PresenterRecievedTestRunMessage()
        {
            if (_isDisabled) return;
            write("Presenter received test run message");
        }

        internal static void PresenterRecievedRunInformationMessage()
        {
            if (_isDisabled) return;
            write("Presenter received run information message");
        }

        internal static void PresenterRecievedInformationMessage()
        {
            if (_isDisabled) return;
            write("Presenter received information message");
        }

        internal static void PresenterRecievedWarningMessage()
        {
            if (_isDisabled) return;
            write("Presenter received warning message");
        }

        internal static void PresenterRecievedErrorMessage()
        {
            if (_isDisabled) return;
            write("Presenter received error message");
        }
    	
		public static void LaunchingEditor(string executable, string arguments)
		{
			if (_isDisabled) return;
			write(string.Format("Launching {0} with {1}", executable, arguments));
		}
    	
		public static void ConfigurationFileMissing()
		{
			write("The configuration file (AutoTest.config) is missing.");
		}

        public static void FailedToConfigure(Exception ex)
        {
            write("Failed to configure application");
            writeException(ex);
        }

        public static void WriteMessage(string message)
        {
            if (_isDisabled) return;
            write(message);
        }

        public static void WriteException(Exception ex)
        {
            if (_isDisabled) return;
            writeException(ex);
        }

        private static void writeException(Exception ex)
        {
            write(string.Format("{1}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace));
        }
    }
}
