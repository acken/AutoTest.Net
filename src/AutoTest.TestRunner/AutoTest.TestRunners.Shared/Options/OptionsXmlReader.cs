using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using System.IO;
using System.Xml;
using AutoTest.TestRunners.Shared.Plugins;

namespace AutoTest.TestRunners.Shared.Options
{
    public class OptionsXmlReader
    {
        private List<Plugin> _plugins = new List<Plugin>();
        private RunOptions _options = null;
        private XmlDocument _xml = null;

        private RunnerOptions _currentRunner = null;
        private AssemblyOptions _currentAssembly = null;

        public IEnumerable<Plugin> Plugins { get { return _plugins; } }
        public RunOptions Options { get { return _options; } }
        public bool IsValid { get; private set; }

        public OptionsXmlReader()
        {
            IsValid = false;
        }

        public void ParseFile(string file)
        {
            if (!File.Exists(file))
            {
                _plugins = null;
                return;
            }

            _options = new RunOptions();
            using (var reader = new XmlTextReader(file))
            {
                while (reader.Read())
                {
                    if (reader.Name.Equals("plugin") && reader.NodeType != XmlNodeType.EndElement)
                        getPlugin(reader);
                    else if (reader.Name.Equals("runner"))
                        getRunner(reader);
                    else if (reader.Name.Equals("ignore_category") && reader.NodeType != XmlNodeType.EndElement)
                        getCategory(reader);
                    else if (reader.Name.Equals("test_assembly"))
                        getAssembly(reader);
                }
            }
            IsValid = true;
        }

		public static TestRunOptions ParseOptions(string xml)
		{
			var options = new TestRunOptions();
			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				while (reader.Read())
				{
					if (reader.Name.Equals("test_run") && reader.NodeType != XmlNodeType.EndElement)
                        getRun(reader, options);
					else if (reader.Name.Equals("test") && reader.NodeType != XmlNodeType.EndElement)
						getTest(reader, options);
					else if (reader.Name.Equals("member") && reader.NodeType != XmlNodeType.EndElement)
						getMember(reader, options);
					else if (reader.Name.Equals("namespace") && reader.NodeType != XmlNodeType.EndElement)
						getNamespace(reader, options);
				}

			}
			return options;
		}

        private static void getNamespace(XmlReader reader, TestRunOptions options)
        {
            reader.Read();
            if (!options.Namespaces.Contains(reader.Value))
                options.AddNamespace(reader.Value);
        }

        private static void getMember(XmlReader reader, TestRunOptions options)
        {
            reader.Read();
            if (!options.Members.Contains(reader.Value))
                options.AddMember(reader.Value);
        }

		private static void getRun(XmlReader reader, TestRunOptions options)
        {
            options.HasBeenVerified(reader.GetAttribute("verified") == "true");
        }

        private void getAssembly(XmlTextReader reader)
        {
			if (reader.IsEmptyElement)
                _currentRunner.AddAssembly(new AssemblyOptions(reader.GetAttribute("name")));
            else if (reader.NodeType == XmlNodeType.EndElement)
                _currentRunner.AddAssembly(_currentAssembly);
            else
                _currentAssembly = new AssemblyOptions(reader.GetAttribute("name"));
        }

        private static void getTest(XmlReader reader, TestRunOptions options)
        {
            reader.Read();
            if (!options.Tests.Contains(reader.Value))
                options.AddTest(reader.Value);
        }

        private void getCategory(XmlTextReader reader)
        {
            reader.Read();
            if (!_currentRunner.Categories.Contains(reader.Value))
                _currentRunner.AddCategory(reader.Value);
        }

        private void getRunner(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
            {
                var id = reader.GetAttribute("id");
                if (_options.TestRuns.Count(x => x.ID.Equals(id)) == 0)
                    _options.AddTestRun(new RunnerOptions(id));
            }
            else if (reader.NodeType == XmlNodeType.EndElement)
            {
                if (_options.TestRuns.Count(x => x.ID.Equals(_currentRunner.ID)) == 0)
                    _options.AddTestRun(_currentRunner);
            }
            else
            {
                var id = reader.GetAttribute("id");
                _currentRunner = _options.TestRuns.FirstOrDefault(x => x.ID.Equals(id));
                if (_currentRunner == null)
                    _currentRunner = new RunnerOptions(id);
            }
        }

        private void getPlugin(XmlTextReader reader)
        {
            var type = reader.GetAttribute("type");
            reader.Read();
            var assembly = reader.Value;
            if (_plugins.FirstOrDefault(x => x.Assembly.Equals(assembly) && x.Type.Equals(type)) == null)
                _plugins.Add(new Plugin(assembly, type));
        }
    }
}
