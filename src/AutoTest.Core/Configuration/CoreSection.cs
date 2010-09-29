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
        
        public ConfigItem<string[]> WatchDirectories { get; private set; }
        public ConfigItem<KeyValuePair<string, string>[]> BuildExecutables { get; private set; }
        public ConfigItem<KeyValuePair<string, string>[]> NUnitTestRunner { get; private set; }
        public ConfigItem<KeyValuePair<string, string>[]> MSTestRunner { get; private set; }
        public ConfigItem<KeyValuePair<string, string>[]> XUnitTestRunner { get; private set; }
        public ConfigItem<CodeEditor> CodeEditor { get; private set; }
        public ConfigItem<bool> DebuggingEnabled { get; private set; }
		public ConfigItem<string> GrowlNotify { get; private set; }
		public ConfigItem<bool> NotifyOnRunStarted { get; private set; }
		public ConfigItem<bool> NotifyOnRunCompleted { get; private set; }
		public ConfigItem<string> WatchIgnoreFile { get; private set; }
		public ConfigItem<string[]> TestAssembliesToIgnore { get; private set; }
		public ConfigItem<string[]> TestCategoriesToIgnore { get; private set; }

        public CoreSection()
        {
			WatchDirectories = new ConfigItem<string[]>(new string[] {});
            BuildExecutables = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string, string>[] {});
            NUnitTestRunner = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string, string>[] {});
            MSTestRunner = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string, string>[] {});
            XUnitTestRunner = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string, string>[] {});
            CodeEditor = new ConfigItem<CodeEditor>(new CodeEditor("", ""));
            DebuggingEnabled = new ConfigItem<bool>(false);
			GrowlNotify = new ConfigItem<string>(null);
			NotifyOnRunStarted = new ConfigItem<bool>(true);
			NotifyOnRunCompleted = new ConfigItem<bool>(true);
			WatchIgnoreFile = new ConfigItem<string>("");
			TestAssembliesToIgnore = new ConfigItem<string[]>(new string[] {});
			TestCategoriesToIgnore = new ConfigItem<string[]>(new string[] {});
        }

        public void Read(string configFile)
        {
            _xml.Load(configFile);
			WatchDirectories = getValues("configuration/DirectoryToWatch");
            BuildExecutables = getVersionedSetting("configuration/BuildExecutable");
            NUnitTestRunner = getVersionedSetting("configuration/NUnitTestRunner");
            MSTestRunner = getVersionedSetting("configuration/MSTestRunner");
            XUnitTestRunner = getVersionedSetting("configuration/XUnitTestRunner");
            CodeEditor = getCodeEditor();
            DebuggingEnabled = getBoolItem("configuration/Debugging", false);
			GrowlNotify = getValueItem("configuration/growlnotify", null);
			NotifyOnRunStarted = getBoolItem("configuration/notify_on_run_started", true);
			NotifyOnRunCompleted = getBoolItem("configuration/notify_on_run_completed", true);
			WatchIgnoreFile = getValueItem("configuration/IgnoreFile", "");
			TestAssembliesToIgnore = getValues("configuration/ShouldIgnoreTestAssembly/Assembly");
			TestCategoriesToIgnore = getValues("configuration/ShouldIgnoreTestCategories/Category");
        }

        private ConfigItem<KeyValuePair<string, string>[]> getVersionedSetting(string xpath)
        {
			var item = new ConfigItem<KeyValuePair<string, string>[]>(new KeyValuePair<string,string>[] {});
            List<KeyValuePair<string, string>> executables = new List<KeyValuePair<string, string>>();
            var nodes = _xml.SelectNodes(xpath);
			if (nodes.Count == 0)
				return item;
            foreach (XmlNode node in nodes)
            {
                var executable = node.InnerText;
                var version = "";
                var attribute = node.SelectSingleNode("@framework");
                if (attribute != null)
                    version = attribute.InnerText;
                executables.Add(new KeyValuePair<string, string>(version, executable));
            }
			item.SetValue(executables.ToArray());
            return item;
        }
		
		private ConfigItem<bool> getBoolItem(string path, bool defaultValue)
        {
			var item = new ConfigItem<bool>(defaultValue);
			var val = getBool(path, defaultValue);
			if (val == null)
				return item;
			item.SetValue((bool) val);
			return item;
        }
		
		private bool? getBool(string nodeName, bool defaultValue)
		{
			bool state;
            var value = getValue(nodeName, "");
            if (bool.TryParse(value, out state))
                return state;
            return defaultValue;
		}

        private ConfigItem<CodeEditor> getCodeEditor()
        {
			var item = new ConfigItem<CodeEditor>(new CodeEditor("", ""));
            var executable = getValue("configuration/CodeEditor/Executable", null);
			if (executable == null)
				return item;
            var arguments = getValue("configuration/CodeEditor/Arguments", "");
            return item.SetValue(new CodeEditor(executable, arguments));
        }

		private ConfigItem<string> getValueItem(string nodeName, string defaultValue)
        {
			var item = new ConfigItem<string>(defaultValue);
            var str = getValue(nodeName, null);
			if (str == null)
				return item;
			item.SetValue(str);
			return item;
        }
		
        private string getValue(string nodeName, string defaultValue)
        {
            var node = _xml.SelectSingleNode(nodeName);
            if (node == null)
                return defaultValue;
            return node.InnerText;
        }

        private ConfigItem<string[]> getValues(string nodeName)
        {
			var item = new ConfigItem<string[]>(new string[] {});
            var values = new List<string>();
            var nodes = _xml.SelectNodes(nodeName);
			if (nodes.Count == 0)
				return item;
            foreach (XmlNode node in nodes)
                values.Add(node.InnerText);
			item.SetValue(values.ToArray());
            return item;
        }
    }
}
