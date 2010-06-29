using System;
using System.Configuration;
using System.IO;
using AutoTest.Core.Messaging;
using System.Collections.Generic;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.Configuration
{
    public class Config : IConfiguration
    {
        private IMessageBus _bus;

        private string _directoryToWatch;
        private List<KeyValuePair<string, string>> _buildExecutables;
        private List<KeyValuePair<string, string>> _nunitTestRunners;
        private List<KeyValuePair<string, string>> _msTestRunner;
        private CodeEditor _codeEditor;
        private bool _debuggingEnabled;

        public Config(IMessageBus bus)
        {
            _bus = bus;
            var core = (CoreSection) ConfigurationManager.GetSection("AutoTestCore");
            _directoryToWatch = core.DirectoryToWatch;
            _buildExecutables = core.BuildExecutables;
            _nunitTestRunners = core.NUnitTestRunner;
            _msTestRunner = core.MSTestRunner;
            _codeEditor = core.CodeEditor;
            _debuggingEnabled = core.DebuggingEnabled;
        }

        public void ValidateSettings()
        {
            if (!Directory.Exists(_directoryToWatch))
                _bus.Publish(new ErrorMessage(string.Format("Invalid watch directory {0}{1}Change the watch directory in the configuration file to a valid directory.", _directoryToWatch, Environment.NewLine)));
            if (noneExists(_buildExecutables))
                _bus.Publish(new WarningMessage("Invalid build executable specified in the configuration file. Builds will not be run."));
            if (noneExists(_nunitTestRunners))
                _bus.Publish(new WarningMessage("NUnit test runner not specified. NUnit tests will not be run."));
            if (noneExists(_msTestRunner))
                _bus.Publish(new WarningMessage("MSTest test runner not specified. MSTest tests will not be run."));
            if (!File.Exists(_codeEditor.Executable))
                _bus.Publish(new WarningMessage("Code editor not specified"));
        }

        private bool noneExists(List<KeyValuePair<string, string>> files)
        {
            foreach (var file in files)
            {
                if (File.Exists(file.Value))
                    return false;
            }
            return true;
        }

        public string DirectoryToWatch
        {
            get { return _directoryToWatch; }
            set { _directoryToWatch = value; }
        }

        public string NunitTestRunner(string version)
        {
            return getVersionedSetting(version, _nunitTestRunners);
        }

        public string MSTestRunner(string version)
        {
            return getVersionedSetting(version, _msTestRunner);
        }

        public CodeEditor CodeEditor
        {
            get { return _codeEditor; }
            set { _codeEditor = value; }
        }

        public bool DebuggingEnabled
        {
            get { return _debuggingEnabled; }
            set { _debuggingEnabled = value; }
        }

        public string BuildExecutable(ProjectDocument project)
        {
            int index;
            if ((index = _buildExecutables.FindIndex(0, b => b.Key.Equals(project.ProductVersion))) >= 0)
                return _buildExecutables[index].Value;
            if ((index = _buildExecutables.FindIndex(0, b => b.Key.Equals(project.Framework))) >= 0)
                return _buildExecutables[index].Value;
            if ((index = _buildExecutables.FindIndex(0, b => b.Key.Equals(""))) >= 0)
                return _buildExecutables[index].Value;
            return _buildExecutables[0].Value;
        }

        private string getVersionedSetting(string version, List<KeyValuePair<string, string>> setting)
        {
            int index;
            if ((index = setting.FindIndex(0, b => b.Key.Equals(version))) >= 0)
                return setting[index].Value;
            if ((index = setting.FindIndex(0, b => b.Key.Equals(""))) >= 0)
                return setting[index].Value;
            return setting[0].Value;
        }
    }
}