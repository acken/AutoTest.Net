using System.Configuration;
using System.IO;
using AutoTest.Core.Messaging;

namespace AutoTest.Core.Configuration
{
    public class Config : IConfiguration
    {
        private IMessageBus _bus;

        private string _directoryToWatch;
        private string _buildExecutable;
        private string _nunitTestRunner;
        private string _msTestRunner;
        private CodeEditor _codeEditor;

        public Config(IMessageBus bus)
        {
            _bus = bus;
            var core = (CoreSection) ConfigurationManager.GetSection("AutoTestCore");
            _directoryToWatch = core.DirectoryToWatch;
            _buildExecutable = core.BuildExecutable;
            _nunitTestRunner = core.NUnitTestRunner;
            _msTestRunner = core.MSTestRunner;
            _codeEditor = core.CodeEditor;
        }

        public void ValidateSettings()
        {
            if (!Directory.Exists(_directoryToWatch))
                _bus.Publish(new ErrorMessage(string.Format("Invalid watch directory {0}\r\nChange the watch directory in the configuration file to a valid directory.", _directoryToWatch)));
            if (!File.Exists(_buildExecutable))
                _bus.Publish(new WarningMessage("Invalid build executable specified in the configuration file. Builds will not be run."));
            if (!File.Exists(_nunitTestRunner))
                _bus.Publish(new WarningMessage("NUnit test runner not specified. NUnit tests will not be run."));
            if (!File.Exists(_msTestRunner))
                _bus.Publish(new WarningMessage("MSTest test runner not specified. MSTest tests will not be run."));
            if (!File.Exists(_codeEditor.Executable))
                _bus.Publish(new WarningMessage("Code editor not specified"));
        }

        public string DirectoryToWatch
        {
            get { return _directoryToWatch; }
            set { _directoryToWatch = value; }
        }

        public string BuildExecutable
        {
            get { return _buildExecutable; }
            set { _buildExecutable = value; }
        }

        public string NunitTestRunner
        {
            get { return _nunitTestRunner; }
            set { _nunitTestRunner = value; }
        }

        public string MSTestRunner
        {
            get { return _msTestRunner; }
            set { _msTestRunner = value; }
        }

        public CodeEditor CodeEditor
        {
            get { return _codeEditor; }
            set { _codeEditor = value; }
        }
    }
}