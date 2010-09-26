using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;

namespace AutoTest.Core.Configuration
{
    class ConfigurationFailureArgument : EventArgs
    {
        public string Message { get; private set; }

        public ConfigurationFailureArgument(string message)
        {
            Message = message;
        }
    }

    class CoreSection
    {
        private XmlDocument _xml = new XmlDocument();
        
        public List<string> WatchDirectories { get; private set; }
        public List<KeyValuePair<string, string>> BuildExecutables { get; private set; }
        public List<KeyValuePair<string, string>> NUnitTestRunner { get; private set; }
        public List<KeyValuePair<string, string>> MSTestRunner { get; private set; }
        public List<KeyValuePair<string, string>> XUnitTestRunner { get; private set; }
        public CodeEditor CodeEditor { get; private set; }
        public bool DebuggingEnabled { get; private set; }
		public string GrowlNotify { get; private set; }
		public bool NotifyOnRunStarted { get; private set; }
		public bool NotifyOnRunCompleted { get; private set; }
		public string WatchIgnoreFile { get; private set; }

        public CoreSection()
        {
            WatchDirectories = new List<string>();
        }

        public void Read(string configFile)
        {
            _xml.Load(configFile);
            WatchDirectories.AddRange(getValues("configuration/DirectoryToWatch"));
            BuildExecutables = getVersionedSetting("configuration/BuildExecutable");
            NUnitTestRunner = getVersionedSetting("configuration/NUnitTestRunner");
            MSTestRunner = getVersionedSetting("configuration/MSTestRunner");
            XUnitTestRunner = getVersionedSetting("configuration/XUnitTestRunner");
            CodeEditor = getCodeEditor();
            DebuggingEnabled = getDebuggingEnabled();
			GrowlNotify = getValue("configuration/growlnotify", null);
			NotifyOnRunStarted = getBool("configuration/notify_on_run_started", true);
			NotifyOnRunCompleted = getBool("configuration/notify_on_run_completed", true);
			WatchIgnoreFile = getValue("configuration/IgnoreFile", "");
        }

        private List<KeyValuePair<string, string>> getVersionedSetting(string xpath)
        {
            List<KeyValuePair<string, string>> executables = new List<KeyValuePair<string, string>>();
            var nodes = _xml.SelectNodes(xpath);
            foreach (XmlNode node in nodes)
            {
                var executable = node.InnerText;
                var version = "";
                var attribute = node.SelectSingleNode("@framework");
                if (attribute != null)
                    version = attribute.InnerText;
                executables.Add(new KeyValuePair<string, string>(version, executable));
            }
            return executables;
        }

        private bool getDebuggingEnabled()
        {
			return getBool("configuration/Debugging", false);
        }
		
		private bool getBool(string nodeName, bool defaultValue)
		{
			bool state;
            var value = getValue(nodeName, "");
            if (bool.TryParse(value, out state))
                return state;
            return defaultValue;
		}

        private CodeEditor getCodeEditor()
        {
            var executable = getValue("configuration/CodeEditor/Executable", "");
            var arguments = getValue("configuration/CodeEditor/Arguments", "");
            return new CodeEditor(executable, arguments);
        }

        private string getValue(string nodeName, string defaultValue)
        {
            var node = _xml.SelectSingleNode(nodeName);
            if (node == null)
                return defaultValue;
            return node.InnerText;
        }

        private string[] getValues(string nodeName)
        {
            var values = new List<string>();
            var nodes = _xml.SelectNodes(nodeName);
            foreach (XmlNode node in nodes)
                values.Add(node.InnerText);
            return values.ToArray();
        }
    }
}
