using System;
using System.IO;
using AutoTest.Core.FileSystem;
namespace AutoTest.Core.Configuration
{
	class DefaultConfigurationLocator : ILocateDefaultConfigurationFile
	{
		public string GetFilePath()
		{
			return Path.Combine(PathParsing.GetRootDirectory(), "AutoTest.config");
		}
	}
}

