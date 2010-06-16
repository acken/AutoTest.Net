using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class MSTestResponseParser
    {
        private List<TestResult> _result = new List<TestResult>();

        public TestRunResults Result { get { return new TestRunResults(_result.ToArray()); } }

        public void ParseLine(string line)
        {
            if (line.StartsWith("Passed"))
            {
                _result.Add(new TestResult(TestStatus.Passed, getTestName(line, "Passed")));
            }
            else if (line.StartsWith("Failed"))
            {
                _result.Add(new TestResult(TestStatus.Failed, getTestName(line, "Failed")));
            }
            else if (line.StartsWith("Ignored"))
            {
                _result.Add(new TestResult(TestStatus.Ignored, getTestName(line, "Ignored")));
            }
            else if (line.StartsWith("Inconclusive"))
            {
                _result.Add(new TestResult(TestStatus.Ignored, getTestName(line, "Inconclusive")));
            }
        }

        private string getTestName(string line, string lineStart)
        {
            return line.Substring(lineStart.Length, line.Length - lineStart.Length).Trim();
        }
    }
}
