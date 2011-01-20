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
	}
}

