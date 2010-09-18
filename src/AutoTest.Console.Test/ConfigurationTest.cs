using System;
using NUnit.Framework;
namespace AutoTest.Console.Test
{
	[TestFixture]
	public class ConfigurationTest
	{
		[Test]
		public void Should_resolve_console()
		{
			ConsoleConfiguration.Configure();
			var console = ConsoleConfiguration.Locate<IConsoleApplication>();
			Assert.IsNotNull(console);
		}
	}
}

