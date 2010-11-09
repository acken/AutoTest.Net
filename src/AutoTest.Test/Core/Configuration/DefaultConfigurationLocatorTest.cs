using System;
using NUnit.Framework;
using AutoTest.Core.Configuration;
using System.IO;
using AutoTest.Core.DebugLog;
namespace AutoTest.Test
{
	[TestFixture]
	public class DefaultConfigurationLocatorTest
	{
		private DefaultConfigurationLocator _locator;
		
		[SetUp]
		public void SetUp()
		{
			_locator = new DefaultConfigurationLocator();
		}
		
		[Test]
		public void Should_return_valid_folder()
		{
			 var configFile = _locator.GetConfigurationFile();
			Directory.Exists(Path.GetDirectoryName(configFile)).ShouldBeTrue();
		}
		
		[Test]
		public void Should_return_debug_logger()
		{
			var logger = _locator.GetDebugLogger();
			logger.ShouldBeOfType<IWriteDebugInfo>();
		}
	}
}

