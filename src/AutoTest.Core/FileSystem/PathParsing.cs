using System;
using System.Reflection;
using System.IO;
namespace AutoTest.Core.FileSystem
{
	public static class PathParsing
	{
        public static string GetRootDirectory()
        {
            return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }
	}
}

