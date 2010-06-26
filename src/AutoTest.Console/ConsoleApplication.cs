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
            configuration.ValidateSettings();
        }

        public void Start()
        {
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

        public void RecievingWarningMessage(WarningMessage message)
        {
            _logger.Warn(message.Warning);
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

        public void RecievingTestRunMessage(TestRunMessage message)
        {
            var assembly = message.Results.Assembly;
            var failed = message.Results.Failed;
            var ignored = message.Results.Ignored;
            if (failed.Length > 0 || ignored.Length > 0)
            {
                _logger.InfoFormat("Test(s) {0} for assembly {1}", failed.Length > 0 ? "failed" : "was ignored", Path.GetFileName(assembly));
                foreach (var test in failed)
                    _logger.InfoFormat("    {0} -> {1}: {2}", test.Status, test.Name, test.Message);
                foreach (var test in ignored)
                    _logger.InfoFormat("    {0} -> {1}: {2}", test.Status, test.Name, test.Message);
            }
        }

        public void RecievingRunStartedMessage(RunStartedMessage message)
        {
            _logger.Info("");
            _logger.Info("Preparing build(s) and test run(s)");
        }

        public void RecievingRunFinishedMessage(RunFinishedMessage message)
        {
            var report = message.Report;
            _logger.InfoFormat(
                "Ran {0} build(s) ({1} succeeded, {2} failed) and {3} test(s) ({4} passed, {5} failed, {6} ignored)",
                report.NumberOfProjectsBuilt,
                report.NumberOfBuildsSucceeded,
                report.NumberOfBuildsFailed,
                report.NumberOfTestsRan,
                report.NumberOfTestsPassed,
                report.NumberOfTestsFailed,
                report.NumberOfTestsIgnored);
        }

        public void RevievingErrorMessage(ErrorMessage message)
        {
            _logger.Info(message.Error);
        }

        public void RecievingRunInformationMessage(RunInformationMessage message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}