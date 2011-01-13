using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;
using System.Xml;

namespace AutoTest.TestRunners.Shared.Results
{
    public class ResultXmlReader
    {
        private string _file;
        private List<TestResult> _results;

        private string _currentRunner = "";
        private string _currentAssembly = "";
        private string _currentFixture = "";
        private TestResult _currentTest;
        private StackLine _currentStackLine;

        public ResultXmlReader(string file)
        {
            _file = file;
        }

        public IEnumerable<TestResult> Read()
        {
            _results = new List<TestResult>();
            try
            {
                using (var reader = new XmlTextReader(_file))
                {
                    while (reader.Read())
                    {
                        if (reader.Name.Equals("runner"))
                            getRunner(reader);
                        else if (reader.Name.Equals("assembly"))
                            getAssembly(reader);
                        else if (reader.Name.Equals("fixture"))
                            getFixture(reader);
                        else if (reader.Name.Equals("test"))
                            getTest(reader);
                        else if (reader.Name.Equals("message") && reader.NodeType != XmlNodeType.EndElement)
                            _currentTest.Message = reader.ReadElementContentAsString();
                        else if (reader.Name.Equals("line"))
                            getStackLine(reader);
                        else if (reader.Name.Equals("method") && reader.NodeType != XmlNodeType.EndElement)
                            _currentStackLine.Method = reader.ReadElementContentAsString();
                        else if (reader.Name.Equals("file") && reader.NodeType != XmlNodeType.EndElement)
                            readFile(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                _results = new List<TestResult>();
                _results.Add(new TestResult("", "", "", "Failed to read AutoTest.TestRunner.exe output xml", TestState.Panic, ex.ToString()));
            }
            return _results;
        }

        private void readFile(XmlTextReader reader)
        {
            int line;
            if (int.TryParse(reader.GetAttribute("line"), out line))
                _currentStackLine.Line = line;
            _currentStackLine.File = reader.ReadElementContentAsString();
        }

        private void getStackLine(XmlTextReader reader)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
                _currentTest.AddStackLine(_currentStackLine);
            else
                _currentStackLine = new StackLine();
        }

        private void getTest(XmlTextReader reader)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
            {
                _results.Add(_currentTest);
            }
            else
            {
                _currentTest = new TestResult();
                _currentTest.Runner = _currentRunner;
                _currentTest.Assembly = _currentAssembly;
                _currentTest.TestFixture = _currentFixture;
                _currentTest.State = getTestState(reader.GetAttribute("state"));
                _currentTest.TestName = reader.GetAttribute("name");
            }
        }

        private TestState getTestState(string state)
        {
            switch (state.ToLower())
            {
                case "failed":
                    return TestState.Failed;
                case "ignored":
                    return TestState.Ignored;
                case "passed":
                    return TestState.Passed;
                case "panic":
                    return TestState.Panic;
            }
            return TestState.Failed;
        }

        private void getFixture(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;
            else if (reader.NodeType == XmlNodeType.EndElement)
                _currentFixture = "";
            else
                _currentFixture = reader.GetAttribute("name");
        }

        private void getAssembly(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;
            else if (reader.NodeType == XmlNodeType.EndElement)
                _currentAssembly = "";
            else
                _currentAssembly = reader.GetAttribute("name");
        }

        private void getRunner(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;
            else if (reader.NodeType == XmlNodeType.EndElement)
                _currentRunner = "";
            else
                _currentRunner = reader.GetAttribute("id");
        }
    }
}
