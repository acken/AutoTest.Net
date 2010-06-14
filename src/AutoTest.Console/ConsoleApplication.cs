using System;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Configuration;
using Castle.Core.Logging;

namespace AutoTest.Console
{
    public class ConsoleApplication : IConsoleApplication
    {
        private readonly IDirectoryWatcher _watcher;
        private readonly IConfiguration _configuration;
        private ILogger _logger;

        public ILogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = NullLogger.Instance;
                return _logger;
            }
            set { _logger = value; }
        }

        public ConsoleApplication(IDirectoryWatcher watcher, IConfiguration configuration)
        {
            _watcher = watcher;
            _configuration = configuration;
        }

        public void Start()
        {
            _logger.InfoFormat("Starting AutoTester and watching \"{0}\" and all subdirectories.", _configuration.DirectoryToWatch);
            _watcher.Watch(_configuration.DirectoryToWatch);
            System.Console.ReadLine();
            Stop();
        }

        public void Stop()
        {
            _watcher.Dispose();
        }
    }
}