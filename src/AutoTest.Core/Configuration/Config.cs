using System.Configuration;

namespace AutoTest.Core.Configuration
{
    public class Config : IConfiguration
    {
        private string _directoryToWatch;
        private string _buildExecutable;
        private string _nunitTestRunner;
        private string _msTestRunner;
        private CodeEditor _codeEditor;


        public Config()
        {
            var core = (CoreSection) ConfigurationManager.GetSection("AutoTestCore");
            _directoryToWatch = core.DirectoryToWatch;
            _buildExecutable = core.BuildExecutable;
            _nunitTestRunner = core.NUnitTestRunner;
            _msTestRunner = core.MSTestRunner;
            _codeEditor = core.CodeEditor;
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