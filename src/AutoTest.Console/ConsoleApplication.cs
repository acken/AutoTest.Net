using System;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Configuration;
using Castle.Core.Logging;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;
using System.IO;

namespace AutoTest.Console
{
    public class ConsoleApplication : IConsoleApplication, IInformationFeedbackView, IRunFeedbackView
    {
        private readonly IDirectoryWatcher _watcher;
        private readonly IConfiguration _configuration;
        private readonly IInformationFeedbackPresenter _informationFeedback;
        private readonly IRunFeedbackPresenter _runFeedback;
        private ILogger _logger;

        public ILogger Logger
        {
            get { if (_logger == null) _logger = NullLogger.Instance; return _logger; }
            set { _logger = value; }
        }

        public ConsoleApplication(IInformationFeedbackPresenter informationFeedback, IRunFeedbackPresenter runFeedbackPresenter, IDirectoryWatcher watcher, IConfiguration configuration)
        {
            _watcher = watcher;
            _configuration = configuration;
            _informationFeedback = informationFeedback;
            _informationFeedback.View = this;
            _runFeedback = runFeedbackPresenter;
            _runFeedback.View = this;
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

        #region IInformationFeedbackView Members

        public void RecievingInformationMessage(InformationMessage message)
        {
            _logger.Info(message.Message);
        }

        #endregion

        #region IRunFeedbackView Members

        public void RecievingBuildMessage(BuildRunMessage runMessage)
        {
            var buildReport = runMessage.Results;
            var project = buildReport.Project;
            if (buildReport.ErrorCount > 0 || buildReport.WarningCount > 0)
            {
                if (buildReport.ErrorCount > 0)
                {
                    _logger.InfoFormat(
                        "Building {0} finished with {1} errors and  {2} warningns",
                        Path.GetFileName(project),
                        buildReport.ErrorCount,
                        buildReport.WarningCount);
                }
                else
                {
                    _logger.InfoFormat(
                        "Building {0} succeeded with {1} warnings",
                        Path.GetFileName(project),
                        buildReport.WarningCount);
                }
                foreach (var error in buildReport.Errors)
                    _logger.InfoFormat("Error: {0}({1},{2}) {3}", error.File, error.LineNumber, error.LinePosition,
                                      error.ErrorMessage);
                foreach (var warning in buildReport.Warnings)
                    _logger.InfoFormat("Warning: {0}({1},{2}) {3}", warning.File, warning.LineNumber,
                                      warning.LinePosition, warning.ErrorMessage);
            }
        }

        #endregion
    }
}