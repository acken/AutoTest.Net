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
        public string BuildExecutable { get; private set; }
        public string NUnitTestRunner { get; private set; }
        public string MSTestRunner { get; private set; }
        public CodeEditor CodeEditor { get; private set; }

        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            _xml.LoadXml(reader.ReadOuterXml());
            DirectoryToWatch = getValue("AutoTestCore/DirectoryToWatch", "");
            BuildExecutable = getValue("AutoTestCore/BuildExecutable", "");
            NUnitTestRunner = getValue("AutoTestCore/NUnitTestRunner", "");
            MSTestRunner = getValue("AutoTestCore/MSTestRunner", "");
            CodeEditor = getCodeEditor();
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
