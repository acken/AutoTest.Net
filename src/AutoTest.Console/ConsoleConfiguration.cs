using System;
using AutoTest.Core.Configuration;
using Castle.Facilities.Logging;
using System.Reflection;
using Castle.MicroKernel.Registration;
namespace AutoTest.Console
{
	public static class ConsoleConfiguration
	{
		public static void Configure()
		{
			BootStrapper.Configure();
            BootStrapper.Container
                .AddFacility("logging", new LoggingFacility(LoggerImplementation.Console));
			BootStrapper.Container.Register(Component.For<IConsoleApplication>().ImplementedBy<ConsoleApplication>());
		}
		
		public static T Locate<T>()
		{
			return BootStrapper.Services.Locate<T>();
		}
	}
}

