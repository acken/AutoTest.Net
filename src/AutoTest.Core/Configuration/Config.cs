using System.Configuration;

namespace AutoTest.Core.Configuration
{
    public class Config : IConfiguration
    {
        private string _directoryToWatch;
        private string _buildExecutable;
        private string _nunitTestRunner;
        private string _msTestRunner;
        private string _ignoreFolder;


        public Config()
        {
            _directoryToWatch = ConfigurationManager.AppSettings["DirectoryToWatch"];
            _buildExecutable = ConfigurationManager.AppSettings["BuildExecutable"];

            _nunitTestRunner = ConfigurationManager.AppSettings["NUnitTestRunner"];
            _msTestRunner = ConfigurationManager.AppSettings["MSTestRunner"];
            
            _ignoreFolder = ConfigurationManager.AppSettings["IgnoreFolder"];
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

        public string IgnoreFolder
        {
            get { return _ignoreFolder; }
            set { _ignoreFolder = value; }
        }
    }
}