using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;

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
        private string _sectionXml = "";

        public string DirectoryToWatch { get; private set; }
        public string BuildExecutable { get; private set; }
        public string NUnitTestRunner { get; private set; }
        public string MSTestRunner { get; private set; }
        public CodeEditor CodeEditor { get; private set; }

        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            _sectionXml = reader.ReadInnerXml();
            DirectoryToWatch = getValue("DirectoryToWatch", "", _sectionXml);
            BuildExecutable = getValue("BuildExecutable", "", _sectionXml);
            NUnitTestRunner = getValue("NUnitTestRunner", "", _sectionXml);
            MSTestRunner = getValue("MSTestRunner", "", _sectionXml);
            CodeEditor = getCodeEditor();
        }

        private CodeEditor getCodeEditor()
        {
            var chunk = getValue("CodeEditor", _sectionXml);
            if (chunk == null)
                return new CodeEditor("", "");
            var executable = getValue("Executable", "", chunk);
            var arguments = getValue("Arguments", "", chunk);
            return new CodeEditor(executable, arguments);
        }

        private string getValue(string nodeName, string defaultValue, string xml)
        {
            var value = getValue(nodeName, xml);
            if (value == null)
                return defaultValue;
            return value;
        }

        private string getValue(string nodeName, string xml)
        {
            var value = getInnerValue(nodeName, xml);
            if (value.Length == 0)
                return null;
            return value;
        }

        private string getInnerValue(string nodeName, string xml)
        {
            var nodeStart = string.Format("<{0}>", nodeName);
            var nodeEnd = string.Format("</{0}>", nodeName);
            var start = xml.IndexOf(nodeStart);
            if (start < 0)
                return null;
            start += nodeStart.Length;
            var end = xml.IndexOf(nodeEnd, start);
            if (end < 0)
                return null;
            return xml.Substring(start, end - start);
        }
    }
}
