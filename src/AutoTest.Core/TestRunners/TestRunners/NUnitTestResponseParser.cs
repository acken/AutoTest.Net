using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;
using System.Xml;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class NUnitTestResponseParser
    {
        private IMessageBus _bus;
        private List<TestResult> _result = new List<TestResult>();
		private List<TestRunResults> _runResults = new List<TestRunResults>();
		private string _content;
		private TestRunInfo[] _testSources;

        public TestRunResults[] Result { get { return _runResults.ToArray(); } }

        public NUnitTestResponseParser(IMessageBus bus)
        {
            _bus = bus;
        }

        public void Parse(string content, TestRunInfo[] runInfos)
        {
			_content = content;
			_testSources = runInfos;
			var testSuites = getTestSuites();
			foreach (var testSuite in testSuites)
			{
				_result.Clear();
	            string[] testCases = getTestCases(testSuite);
	            foreach (var testCase in testCases)
	            {
	                string name = getname(testCase);
	
	                var status = TestStatus.Passed;
	                if (testCase.Contains("executed=\"False\""))
	                    status = TestStatus.Ignored;
	                else if (testCase.Contains("success=\"False\""))
	                    status = TestStatus.Failed;
	
	                string message = "";
	                if (status.Equals(TestStatus.Ignored))
	                    message = getMessage(testCase);
	                else if (status.Equals(TestStatus.Failed))
	                    message = getMessage(testCase);
	
	                IStackLine[] stackTrace = new IStackLine[] {};
	                if (status.Equals(TestStatus.Failed))
	                    stackTrace = getStackTrace(testCase);
	                _result.Add(new TestResult(status, name, message, stackTrace));
	            }
				var runInfo = matchToTestSource(testSuite);
				if (runInfo ==  null)
					continue;
				var results = new TestRunResults(runInfo.Project.Key, runInfo.Assembly, _result.ToArray());
				results.SetTimeSpent(getTimeSpent(testSuite));
				_runResults.Add(results);
			}
        }
		
		private TestRunInfo matchToTestSource(string testSuite)
		{
			var assembly = getAssemblyName(testSuite);
			foreach (var source in _testSources)
			{
				if (source.Assembly.Substring(source.Assembly.Length - assembly.Length, assembly.Length).Equals(assembly))
					return source;
			}
			return null;
		}
		
		private string getAssemblyName(string testSuite)
		{
			var start = testSuite.IndexOf("name=\"");
			if (start == -1)
				return "";
			start += "name=\"".Length;
			var end = testSuite.IndexOf("\"", start);
			if (end == -1)
				return "";
			return testSuite.Substring(start, end - start);
		}
		
		private TimeSpan getTimeSpent(string testSuite)
		{
			var start = testSuite.IndexOf("time=\"");
			if (start == -1)
				return new TimeSpan(0);
			start += "time=\"".Length;
			var end = testSuite.IndexOf("\"", start);
			if (end == -1)
				return new TimeSpan(0);
			var time = testSuite.Substring(start, end - start);
			var chunks = time.Split(new char[] { '.' });
			if (chunks.Length != 2)
				return new TimeSpan(0);
			int seconds, milliseconds;
			if (!int.TryParse(chunks[0], out seconds))
				return new TimeSpan(0);
			if (!int.TryParse(chunks[1], out milliseconds))
				return new TimeSpan(0);
			return new TimeSpan(0, 0, 0, seconds, milliseconds);
		}
		
		private string[] getTestSuites()
		{
			var mainTestSuite = getMainTestSuite();
			if (singleAssemblyTestRun())
				return new string[] { mainTestSuite };
			var subSuites = getSubTestSuites(mainTestSuite);
			if (subSuites.Length == 0)
				return new string[] { mainTestSuite };
			return subSuites;
		}
		
		private string getMainTestSuite()
		{
			int start = _content.IndexOf("<test-suite ");
			if (start == -1)
				return "";
			int end = _content.LastIndexOf("</test-suite>");
			if (end == -1)
				return "";
			end += "</test-suite>".Length;
			return _content.Substring(start, end - start);
		}
		
		private bool singleAssemblyTestRun()
		{
			return _content.IndexOf("<test-suite name=\"UNNAMED\"") == -1;
		}
		
		private string[] getSubTestSuites(string mainTestSuite)
		{
			var subSuites = new List<string>();
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(mainTestSuite);
			var nodes = xmlDocument.SelectNodes("test-suite/results/test-suite");
			foreach (XmlNode node in nodes)
				subSuites.Add(node.OuterXml);
			return subSuites.ToArray();
		}

        private string[] getTestCases(string content)
        {
            int start = 0;
            List<string> testCases = new List<string>();
            do
            {
                start = getTestCaseStart(content, start);
                if (start < 0)
                    continue;
                int end = getTestCaseEnd(content, start);
                if (end < 0)
                    break;
                testCases.Add(content.Substring(start, end - start));
                start = end;
            } while (start >= 0);
            return testCases.ToArray();
        }

        private int getTestCaseStart(string content, int start)
        {
            return content.IndexOf("<test-case", start, StringComparison.CurrentCultureIgnoreCase);
        }

        private int getTestCaseEnd(string content, int start)
        {
            int selfClosedEnd = content.IndexOf("/>", start);
            int endTag = content.IndexOf("</test-case>", start, StringComparison.CurrentCultureIgnoreCase);
            if (selfClosedEnd < 0 && endTag < 0)
            {
                _bus.Publish(new WarningMessage(string.Format("Invalid NUnit response format. Could not find <testcase> closing tag for {0}",
                                   content)));
                return -1;
            }

            int end;
            if (selfClosedEnd == -1 || (endTag > 0 && endTag < selfClosedEnd))
                end = endTag + "</test-case>".Length;
            else
                end = selfClosedEnd + "/>".Length;
            return end;
        }

        private string getname(string testCase)
        {
            string tagStart = "name=\"";
            string tagEnd = "\"";
            return getStringContent(testCase, tagStart, tagEnd).Trim();
        }

        private string getMessage(string testCase)
        {
            string tagStart = "<message><![CDATA[";
            string tagEnd = "]]></message>";
            return getStringContent(testCase, tagStart, tagEnd).TrimEnd();
        }

        private IStackLine[] getStackTrace(string testCase)
        {
            var tagStart = "<stack-trace><![CDATA[";
            var tagEnd = "]]></stack-trace>";
            var lines = getStringContent(testCase, tagStart, tagEnd).TrimEnd().Split(new string[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            List<IStackLine> stackLines = new List<IStackLine>();
            foreach (var line in lines)
                stackLines.Add(new NUnitStackLine(line));
            return stackLines.ToArray();
        }

        private string getStringContent(string testCase, string tagStart, string tagEnd)
        {
            int start = testCase.IndexOf(tagStart, 0, StringComparison.CurrentCultureIgnoreCase) + tagStart.Length;
            if (start < tagStart.Length)
                return "";
            int end = testCase.IndexOf(tagEnd, start, StringComparison.CurrentCultureIgnoreCase);
            if (end < 0)
                return "";
            return testCase.Substring(start, end - start);
        }
    }
}
