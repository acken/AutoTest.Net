using System;
using System.Configuration;
using System.IO;
using AutoTest.Core.Messaging;
using System.Collections.Generic;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.FileSystem;
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Configuration
{
    public class Config : IConfiguration
    {
        private IMessageBus _bus;

        private string[] _watchDirectories;
        private List<KeyValuePair<string, string>> _buildExecutables;
        private List<KeyValuePair<string, string>> _nunitTestRunners;
        private List<KeyValuePair<string, string>> _msTestRunner;
        private List<KeyValuePair<string, string>> _xunitTestRunner;
        private CodeEditor _codeEditor;
        private bool _debuggingEnabled;

        public Config(IMessageBus bus)
        {
            _bus = bus;
			var core = getConfiguration();
            tryToConfigure(core);
        }

        private void tryToConfigure(CoreSection core)
        {
            try
            {
                _watchDirectories = core.WatchDirectories.ToArray();
                _buildExecutables = core.BuildExecutables;
                _nunitTestRunners = core.NUnitTestRunner;
                _msTestRunner = core.MSTestRunner;
                _xunitTestRunner = core.XUnitTestRunner;
                _codeEditor = core.CodeEditor;
                _debuggingEnabled = core.DebuggingEnabled;
            }
            catch (Exception ex)
            {
                DebugLog.Debug.FailedToConfigure(ex);
                throw;
            }
        }
		
		private CoreSection getConfiguration()
		{
			var core = new CoreSection();
			var configFile = Path.Combine(PathParsing.GetRootDirectory(), "AutoTest.config");
			if (!File.Exists(configFile))
			{
				Debug.ConfigurationFileMissing();
				return core;
			}
			core.Read(configFile);
			return core;
		}

        public void ValidateSettings()
        {
            if (noneExists(_buildExecutables))
                _bus.Publish(new WarningMessage("Invalid build executable specified in the configuration file. Builds will not be run."));
            if (noneExists(_nunitTestRunners))
                _bus.Publish(new WarningMessage("NUnit test runner not specified. NUnit tests will not be run."));
            if (noneExists(_msTestRunner))
                _bus.Publish(new WarningMessage("MSTest test runner not specified. MSTest tests will not be run."));
			if (noneExists(_xunitTestRunner))
                _bus.Publish(new WarningMessage("XUnit test runner not specified. XUnit tests will not be run."));
            if (_codeEditor == null || !File.Exists(_codeEditor.Executable))
                _bus.Publish(new WarningMessage("Code editor not specified"));
        }

        private bool noneExists(List<KeyValuePair<string, string>> files)
        {
			if (files == null)
				return true;
			
            foreach (var file in files)
            {
                if (File.Exists(file.Value))
                    return false;
            }
            return true;
        }

        public string[] WatchDirectores
        {
            get { return _watchDirectories; }
            set { _watchDirectories = value; }
        }

        public string NunitTestRunner(string version)
        {
            return getVersionedSetting(version, _nunitTestRunners);
        }

        public string MSTestRunner(string version)
        {
            return getVersionedSetting(version, _msTestRunner);
        }

        public string XunitTestRunner(string version)
        {
            return getVersionedSetting(version, _xunitTestRunner);
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
            if (_buildExecutables.Count == 0)
                return "";
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
            if (setting.Count == 0)
                return "";
            int index;
            if ((index = setting.FindIndex(0, b => b.Key.Equals(version))) >= 0)
                return setting[index].Value;
            if ((index = setting.FindIndex(0, b => b.Key.Equals(""))) >= 0)
                return setting[index].Value;
            return setting[0].Value;
        }
    }
}