using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace AutoTest.Core.ForeignLanguageProviders.Php
{
	public class PhpFileConventionMatcher
	{
		private string _token;
		private string _pattern;
		private string[] _testLocations;

		public PhpFileConventionMatcher(string token, string pattern, string[] testLocations)
		{
			if (Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX) {
				_token = token.Replace("/", "\\");
				_pattern = pattern.Replace("/", "\\");
				_testLocations = testLocations.Select(x => x.Replace("/", "\\")).ToArray();
				return;
			}
			_token = token;
			_pattern = pattern;
			_testLocations = testLocations;
		}

		public string[] Match(string file)
		{
			if (file.Length < _pattern.Length)
				return new string[] {};
			var start = file.IndexOf(_pattern, _token.Length);
			if (start == -1)
				return new string[] {};
			start += _pattern.Length;
			var locations = new List<string>();
			foreach (var location in _testLocations) {
				if (!location.EndsWith("*")) {
					locations.Add(Path.Combine(file.Substring(0, start), location));
					continue;
				}
				locations.Add(
					Path.GetDirectoryName(
						Path.Combine(
							Path.Combine(
								file.Substring(0, start),
								location.Trim(new[] {'*'})),
							file.Substring(start, file.Length - start))));
			}
			return locations.ToArray();
		}
	}
}