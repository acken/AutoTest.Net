using AutoTest.Core.Configuration;
using log4net;
using log4net.Config;
using System.Reflection;

[assembly: XmlConfigurator(Watch = true)]

namespace AutoTest.Console
{
    internal class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _logger.Debug("Starting up AutoTester");
            BootStrapper.Configure();
            BootStrapper.RegisterAssembly(Assembly.GetExecutingAssembly());
            BootStrapper.InitializeCache();
            var application = BootStrapper.Services.Locate<IConsoleApplication>();
            application.Start();
        }
    }
}