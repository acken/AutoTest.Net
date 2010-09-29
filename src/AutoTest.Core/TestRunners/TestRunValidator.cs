using System;
using AutoTest.Core.Configuration;
namespace AutoTest.Core.TestRunners
{
	class TestRunValidator : IDetermineIfAssemblyShouldBeTested
	{
		private IConfiguration _configuration;
		
		public TestRunValidator(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		
		#region IDetermineIfAssemblyShouldBeTested implementation
		public bool ShouldNotTestAssembly(string assembly)
		{
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

