using AutoTest.Core.Configuration;
using System.Reflection;
using Castle.Core.Logging;
using log4net.Config;
using log4net;
using Castle.Facilities.Logging;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace AutoTest.Console
{
    internal class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _logger.Info("Starting up AutoTester");
            BootStrapper.Configure();
            BootStrapper.Container
                .AddFacility("logging", new LoggingFacility(LoggerImplementation.Log4net));
            BootStrapper.RegisterAssembly(Assembly.GetExecutingAssembly());
            BootStrapper.InitializeCache();
            var application = BootStrapper.Services.Locate<IConsoleApplication>();
            application.Start();
        }
    }
}