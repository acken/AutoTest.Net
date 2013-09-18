using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AutoTest.Messages;
using AutoTest.Core.Messaging;
using AutoTest.Core.ForeignLanguageProviders.Php;
using AutoTest.Core.Configuration;

namespace AutoTest.Core.ForeignLanguageProviders.Php
{
	public class PhpRunHandler
	{
        private IMessageBus _bus;
        private IConfiguration _config;

        public bool IsRunning { get; private set; }

        public PhpRunHandler(IMessageBus bus, IConfiguration config)
        {
        	_bus = bus;
            _config = config;
        }

        public void Handle(List<ChangedFile> files)
        {
        	IsRunning = true;
            _bus.Publish(new RunStartedMessage(files.ToArray()));
            var runReport = new RunReport();

            var patterns = _config.AllSettings("php.Convention.Pattern");
            var testPaths = _config.AllSettings("php.Convention.TestPaths");

            var testLocations = new List<string>();
            if (patterns.Length == 0 && testPaths.Length == 0) {
                testLocations.Add("");
            } else {
                var matcher = new PhpFileConventionMatcher(
                                    _config.WatchPath,
                                    patterns.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                                    testPaths.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries));
                foreach (var file in files) {
                    foreach (var location in matcher.Match(file.FullName)) {
                        if (!Directory.Exists(location))
                            continue;
                        if (!testLocations.Contains(location))
                            testLocations.Add(location);
                    }
                }
            }

            foreach (var location in testLocations) {
                var results 
                    = new PhpUnitRunner()
                    .Run(
                        "-c app " + location,
                        _config.WatchPath,
                        (line) => {
                            sendLiveFeedback(line);
                        });
                AutoTest.Core.DebugLog.Debug.WriteDebug("Returned " + results.Count.ToString() + " results");
                foreach (var result in results) {
                    AutoTest.Core.DebugLog.Debug.WriteDebug("Result contains " + result.All.Length.ToString() + " tests");
                    runReport.AddTestRun(
                        result.Project,
                        result.Assembly,
                        result.TimeSpent,
                        result.Passed.Length,
                        result.Ignored.Length,
                        result.Failed.Length);
                    _bus.Publish(new TestRunMessage(result));
                }
            }
            // Oh my god.. please fix this
            // Issue with ordering of TestRunMessage and RunFinishedMessage
            System.Threading.Thread.Sleep(100);
            _bus.Publish(new RunFinishedMessage(runReport));
            IsRunning = false;
        }

        public void Abort() 
        {
        }

        private void sendLiveFeedback(string line) {
            var parser = new PhpUnitLiveParser();
            if (!parser.Parse(line))
                return;
            _bus.Publish(
                new LiveTestStatusMessage(
                    parser.Class,
                    parser.Test,
                    -1,
                    parser.TestsCompleted,
                    new LiveTestStatus[] {},
                    new LiveTestStatus[] {}));
        }
	}
}