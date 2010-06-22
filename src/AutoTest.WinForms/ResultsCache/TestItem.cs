using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;

namespace AutoTest.WinForms.ResultsCache
{
    class TestItem : IItem
    {
        public string Key { get; private set; }
        public string Project { get; private set; }
        public TestResult Value { get; private set; }

        public TestItem(string key, string project, TestResult value)
        {
            Key = key;
            Project = project;
            Value = value;
        }

        public override bool  Equals(object obj)
        {
            var other = (TestItem) obj;
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}", Key, Value.GetHashCode().ToString()).GetHashCode();
        }

        public override string ToString()
        {
            var stackTrace = new StringBuilder();
            var stackLines = Value.StackTrace.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in stackLines)
            {
                if (line.IndexOf(") in ") > 0)
                {
                    stackTrace.AppendLine(string.Format("{0}{1}",
                                                        line.Replace(") in ",
                                                                     string.Format(") in {0}", LinkParser.TAG_START)),
                                                        LinkParser.TAG_END));
                }
                else
                {
                    stackTrace.AppendLine(line);
                }
            }
            return string.Format(
                "Assembly: {0}\r\n" +
                "Test: {1}\r\n" +
                "Message:\r\n{2}\r\n" +
                "Stack trace\r\n{3}",
                Key,
                Value.Name,
                Value.Message,
                stackTrace.ToString());
        }

        #region IItem Members


        public void HandleLink(string link)
        {
            var file = link.Substring(0, link.IndexOf(":line"));
            var lineNumber = getLineNumber(link);
            var launcher = new ApplicatonLauncher(file, lineNumber);
            launcher.Launch();
        }

        private int getLineNumber(string link)
        {
            var start = link.IndexOf(":line");
            if (start < 0)
                return 0;
            start += ":line".Length;
            return int.Parse(link.Substring(start, link.Length - start));
        }

        #endregion
    }
}
