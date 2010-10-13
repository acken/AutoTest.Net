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
        private string _lastParsed = "";
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
                addResult(line, TestRunStatus.Passed, "Passed");
            }
            else if (line.StartsWith("Failed"))
            {
                addResult(line, TestRunStatus.Failed, "Failed");
            }
            else if (line.StartsWith("Ignored"))
            {
                addResult(line, TestRunStatus.Ignored, "Ignored");
            }
            else if (line.StartsWith("Inconclusive"))
            {
                addResult(line, TestRunStatus.Ignored, "Inconclusive");
            }
            else if (line.StartsWith("[errormessage] =") && _result.Count > 0)
            {
                _result[_result.Count - 1].Message = getChunk(line, "[errormessage] =");
                _lastParsed = "[errormessage] =";

            }
            else if (line.StartsWith("[errorstacktrace] =") && _result.Count > 0)
            {
                _result[_result.Count - 1].StackTrace = getStackTrace(line, "[errorstacktrace] =");
                _lastParsed = "[errorstacktrace] =";
            }
            else if (_lastParsed.Equals("[errormessage] ="))
            {
                _result[_result.Count - 1].Message += string.Format("{0}{1}", Environment.NewLine, line);
            }
            else if (_lastParsed.Equals("[errorstacktrace] ="))
            {
                _result[_result.Count - 1].StackTrace = getStackTrace(line);
            }
        }

        private void addResult(string line, TestRunStatus status, string lineStart)
        {
            _result.Add(new TestResult(status, getChunk(line, lineStart)));
            _lastParsed = lineStart;
        }

        private IStackLine[] getStackTrace(string line)
        {
            return getStackTrace(line, null);
        }

        private IStackLine[] getStackTrace(string line, string lineStart)
        {
            List<IStackLine> stackLines = new List<IStackLine>();
            if (_result[_result.Count - 1].StackTrace != null)
                stackLines.AddRange(_result[_result.Count - 1].StackTrace);
            var stackString = lineStart == null ? line : getChunk(line, lineStart);
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
