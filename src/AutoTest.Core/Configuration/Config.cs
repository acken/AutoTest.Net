using System.Configuration;

namespace AutoTest.Core.Configuration
{
    public class Config : IConfiguration
    {
        private string _directoryToWatch;
        private string _buildExecutable;
        private string _unitTestExe;
        private string _ignoreFolder;


        public Config()
        {
            _directoryToWatch = ConfigurationManager.AppSettings["DirectoryToWatch"];

            _buildExecutable = ConfigurationManager.AppSettings["BuildExecutable"];

            _unitTestExe = ConfigurationManager.AppSettings["UnitTestExe"];
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

        public string UnitTestExe
        {
            get { return _unitTestExe; }
            set { _unitTestExe = value; }
        }

        public string IgnoreFolder
        {
            get { return _ignoreFolder; }
            set { _ignoreFolder = value; }
        }
    }
}