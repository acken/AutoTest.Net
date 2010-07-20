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

    class CoreSection : ConfigurationSection
    {
        private XmlDocument _xml = new XmlDocument();

        public string DirectoryToWatch { get; private set; }
        public List<KeyValuePair<string, string>> BuildExecutables { get; private set; }
        public List<KeyValuePair<string, string>> NUnitTestRunner { get; private set; }
        public List<KeyValuePair<string, string>> MSTestRunner { get; private set; }
        public List<KeyValuePair<string, string>> XUnitTestRunner { get; private set; }
        public CodeEditor CodeEditor { get; private set; }
        public bool DebuggingEnabled { get; private set; }

        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            _xml.LoadXml(reader.ReadOuterXml());
            DirectoryToWatch = getValue("AutoTestCore/DirectoryToWatch", "");
            BuildExecutables = getVersionedSetting("AutoTestCore/BuildExecutable");
            NUnitTestRunner = getVersionedSetting("AutoTestCore/NUnitTestRunner");
            MSTestRunner = getVersionedSetting("AutoTestCore/MSTestRunner");
            XUnitTestRunner = getVersionedSetting("AutoTestCore/XUnitTestRunner");
            CodeEditor = getCodeEditor();
            DebuggingEnabled = getDebuggingEnabled();
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
            bool state;
            var value = getValue("AutoTestCore/Debugging", "false");
            if (bool.TryParse(value, out state))
                return state;
            return false;
        }

        private CodeEditor getCodeEditor()
        {
            var executable = getValue("AutoTestCore/CodeEditor/Executable", "");
            var arguments = getValue("AutoTestCore/CodeEditor/Arguments", "");
            return new CodeEditor(executable, arguments);
        }

        private string getValue(string nodeName, string defaultValue)
        {
            var node = _xml.SelectSingleNode(nodeName);
            if (node == null)
                return defaultValue;
            return node.InnerText;
        }
    }
}
