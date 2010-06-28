using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.TestRunners.TestRunners
{
    class MSTestResponseParser
    {
        private string _project;
        private string _assembly;
        private List<TestResult> _result = new List<TestResult>();

        public TestRunResults Result { get { return new TestRunResults(_project, _assembly, _result.ToArray()); } }

        public MSTestResponseParser(string project, string assembly)
        {
            _project = project;
            _assembly = assembly;
        }

        public void ParseLine(string line)
        {
            if (line.StartsWith("Passed"))
            {
                _result.Add(new TestResult(TestStatus.Passed, getChunk(line, "Passed")));
            }
            else if (line.StartsWith("Failed"))
            {
                _result.Add(new TestResult(TestStatus.Failed, getChunk(line, "Failed")));
            }
            else if (line.StartsWith("Ignored"))
            {
                _result.Add(new TestResult(TestStatus.Ignored, getChunk(line, "Ignored")));
            }
            else if (line.StartsWith("Inconclusive"))
            {
                _result.Add(new TestResult(TestStatus.Ignored, getChunk(line, "Inconclusive")));
            }
            else if (line.StartsWith("[errormessage] =") && _result.Count > 0)
            {
                _result[_result.Count - 1].Message = getChunk(line, "[errormessage] =");
            }
            else if (line.StartsWith("[errorstacktrace] =") && _result.Count > 0)
            {
                _result[_result.Count - 1].StackTrace = getStackTrace(line, "[errorstacktrace] =");
            }
        }

        private IStackLine[] getStackTrace(string line, string lineStart)
        {
            List<IStackLine> stackLines = new List<IStackLine>();
            if (_result[_result.Count - 1].StackTrace != null)
                stackLines.AddRange(_result[_result.Count - 1].StackTrace);
            var stackString = getChunk(line, lineStart);
            var lines = stackString.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var stackLine in lines)
                stackLines.Add(new MSTestStackLine(stackLine));
            return stackLines.ToArray();
        }

        private string getChunk(string line, string lineStart)
        {
            return line.Substring(lineStart.Length, line.Length - lineStart.Length).Trim();
        }
    }
}
