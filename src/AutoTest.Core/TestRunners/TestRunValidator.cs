using System;
using AutoTest.Core.Configuration;
using AutoTest.Core.FileSystem;
namespace AutoTest.Core.TestRunners
{
	class TestRunValidator : IDetermineIfAssemblyShouldBeTested
	{
		private IConfiguration _configuration;
        private IFileSystemService _fs;

        public TestRunValidator(IConfiguration configuration, IFileSystemService fs)
		{
			_configuration = configuration;
            _fs = fs;
		}
		
		#region IDetermineIfAssemblyShouldBeTested implementation
		public bool ShouldNotTestAssembly(string assembly)
		{
            if (!_fs.FileExists(assembly))
                return false;

			foreach (var pattern in _configuration.TestAssembliesToIgnore)
			{
				if (pattern.StartsWith("*") && pattern.EndsWith("*") && assembly.Contains(pattern.Substring(1, pattern.Length - 2)))
					return true;
				if (pattern.StartsWith("*") && assembly.EndsWith(pattern.Substring(1, pattern.Length - 1)))
					return true;
				if (pattern.EndsWith("*") && assembly.StartsWith(pattern.Substring(0, pattern.Length - 1)))
					return true;
				if (pattern.Equals(assembly))
					return true;
			}
			return false;
		}
		#endregion
	}
}

