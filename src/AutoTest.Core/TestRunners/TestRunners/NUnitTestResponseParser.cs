using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using AutoTest.Core.Messaging;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class NUnitTestResponseParser
    {
        private IMessageBus _bus;
        private string _project;
        private string _assembly;
        private List<TestResult> _result = new List<TestResult>();
		private List<TestRunResults> _runResults = new List<TestRunResults>();

        public TestRunResults[] Result { get { return _runResults.ToArray(); } }

        public NUnitTestResponseParser(IMessageBus bus, string project, string assembly)
        {
            _bus = bus;
            _project = project;
            _assembly = assembly;
        }

        public void Parse(string content)
        {
            string[] testCases = getTestCases(content);
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
			_runResults.Add(new TestRunResults(_project, _assembly, _result.ToArray()));
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
