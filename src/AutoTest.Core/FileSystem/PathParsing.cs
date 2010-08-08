using System;
using System.Reflection;
using System.IO;
namespace AutoTest.Core.FileSystem
{
	public static class PathParsing
	{
		public static string GetRootDirectory()
		{
			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
			if (isUnixStyleFileURI(path))
			    path = convertFromUnixStyleFileURI(path);
			return path;
		}
		
		private static bool isUnixStyleFileURI(string path)
		{
			return path.StartsWith("file:");
		}
		
		private static string convertFromUnixStyleFileURI(string path)
		{
			return path.Substring(5, path.Length - 5);
		}
	}
}

