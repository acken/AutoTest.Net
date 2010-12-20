using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using AutoTest.Core.Configuration;

namespace AutoTest.Core.FileSystem
{
    class WatchValidator : IWatchValidator
    {
		private IConfiguration _configuration;
		private string[] _defaultIgnores = new string[11];

		public WatchValidator(IConfiguration configuration)
		{
			_configuration = configuration;
			_defaultIgnores[0] = "bin/Debug";
			_defaultIgnores[1] = "bin/Release";
			_defaultIgnores[2] = "bin/AutoTest.Net";
			_defaultIgnores[3] = "bin/x86";
			_defaultIgnores[4] = "obj/Debug";
			_defaultIgnores[5] = "obj/Release";
			_defaultIgnores[6] = "obj/x86";
			_defaultIgnores[7] = "*.FileListAbsolute.txt";
			_defaultIgnores[8] = "*.FilesWrittenAbsolute.txt";
			_defaultIgnores[9] = "*.suo";
            _defaultIgnores[10] = "*.UnmanagedRegistration.cache";
		}
		
        public bool ShouldPublish(string filePath)
        {
			if (!_configuration.ShouldUseIgnoreLists)
				return true;
			filePath = filePath.Replace("\\", "/");
			if (match(filePath, _defaultIgnores))
				return false;
			if (match(filePath, _configuration.WatchIgnoreList))
				return false;
            return true;
        }
		
		public string GetIgnorePatterns()
		{
			var list = "";
			foreach (var pattern in _configuration.WatchIgnoreList)
				list += (list.Length == 0 ? "" : "|") + pattern;
			if (_configuration.CustomOutputPath != null && _configuration.CustomOutputPath.Length > 0)
				list += (list.Length == 0 ? "" : "|") + _configuration.CustomOutputPath;
			return list;
		}
		
		private bool match(string stringToMatch, string[] patterns)
		{
			foreach (var patter in patterns)
			{
				if (patter.EndsWith(Path.DirectorySeparatorChar.ToString()))
				{
					if (stringToMatch.Contains(patter))
						return true;
				}
				if (stringToMatch.EndsWith(patter))
					return true;
				if (stringToMatch.Contains(string.Format("{0}{1}{0}", Path.DirectorySeparatorChar, patter)))
					return true;
				if (matchStringToGlobUsingGlobishMatching(stringToMatch, patter))
					return true;
			}
			return false;
		}

        private bool contains(string path, string stringToSearchFor)
        {
            return path.IndexOf(stringToSearchFor) >= 0;
        }
			
	    private bool matchStringToGlobUsingGlobishMatching(string stringToMatch, string pattern)
	    {
	        string regExPattern = Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".");
	        var regex = new Regex(regExPattern, RegexOptions.Singleline);
			return regex.IsMatch(stringToMatch);
		}
    }
}
