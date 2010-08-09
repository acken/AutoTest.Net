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
			if (isURI(path))
			    path = convertFromURI(path);
			return path;
		}
		
		private static bool isURI(string path)
		{
			return path.StartsWith("file:");
		}
		
		private static string convertFromURI(string path)
		{
            var converted = path.Substring(6, path.Length - 6);
            if (!Path.IsPathRooted(converted))
                converted = string.Format("{0}{1}", Path.VolumeSeparatorChar, converted);
            return converted;
		}
	}
}

