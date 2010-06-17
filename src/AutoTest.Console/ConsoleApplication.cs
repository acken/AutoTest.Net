using System;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Configuration;
using Castle.Core.Logging;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;

namespace AutoTest.Console
{
    public class ConsoleApplication : IConsoleApplication, IFeedbackView
    {
        private readonly IDirectoryWatcher _watcher;
        private readonly IConfiguration _configuration;
        private readonly IFeedbackPresenter _feedback;
        private ILogger _logger;

        public ILogger Logger
        {
            get { if (_logger == null) _logger = NullLogger.Instance; return _logger; }
            set { _logger = value; }
        }

        public ConsoleApplication(IFeedbackPresenter presenter, IDirectoryWatcher watcher, IConfiguration configuration)
        {
            _watcher = watcher;
            _configuration = configuration;
            _feedback = presenter;
            _feedback.View = this;
        }

        public void Start()
        {
            Logger.InfoFormat("Starting AutoTester and watching \"{0}\" and all subdirectories.", _configuration.DirectoryToWatch);
            _watcher.Watch(_configuration.DirectoryToWatch);
            System.Console.ReadLine();
            Stop();
        }

        public void Stop()
        {
            _watcher.Dispose();
        }

        #region IFeedbackView Members

        public void RecievingInformationMessage(InformationMessage message)
        {
            _logger.Info(message.Message);
        }

        #endregion
    }
}