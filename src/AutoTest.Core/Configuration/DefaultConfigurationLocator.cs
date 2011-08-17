using System;
using System.IO;
using AutoTest.Core.FileSystem;
using AutoTest.Core.DebugLog;
using AutoTest.Core.Messaging;
namespace AutoTest.Core.Configuration
{
	class DefaultConfigurationLocator : ILocateWriteLocation
	{
        public string GetLogfile()
        {
            return Path.Combine(getPath(), "debug.log");
        }

		public string GetConfigurationFile()
		{
			return Path.Combine(getPath(), "AutoTest.config");
		}
		
		private string getPath()
		{
			return PathParsing.GetRootDirectory();
		}

        private string getLogPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var atDir = Path.Combine(appData, "AutoTest.Net");
            if (!Directory.Exists(atDir))
                Directory.CreateDirectory(atDir);
            return atDir;
        }
	}
}

