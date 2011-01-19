using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;
using System.IO;
using AutoTest.Core.Launchers;
using AutoTest.Core.Configuration;
using AutoTest.Messages;

namespace AutoTest.Core.Caching.RunResultCache
{
    public class TestItem : IItem
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
            foreach (var line in Value.StackTrace)
            {
				DebugLog.Debug.WriteDetail("Stack line message " + line.File);
                if (File.Exists(line.File))
                {
                    stackTrace.AppendLine(string.Format("at {0} in {1}{2}:line {3}{4}",
                                                        line.Method,
                                                        LinkParser.TAG_START,
                                                        line.File,
                                                        line.LineNumber,
                                                        LinkParser.TAG_END));
                }
                else
                {
                    stackTrace.AppendLine(line.ToString());
                }

            }
            return string.Format(
                "Assembly: {0}{4}" +
                "Test: {1}{4}" +
                "Message:{4}{2}{4}" +
                "Stack trace{4}{3}",
                Key,
                Value.Name,
                Value.Message,
                stackTrace.ToString(),
			    Environment.NewLine);
        }

        public bool IsTheSameTestAs(TestItem item)
        {
            return Key.Equals(item.Key) && Value.Runner.Equals(item.Value.Runner) && Value.Name.Equals(item.Value.Name);
        }

        #region IItem Members


        public void HandleLink(string link)
        {
            var file = link.Substring(0, link.IndexOf(":line"));
            var lineNumber = getLineNumber(link);
            var launcher = BootStrapper.Services.Locate<ApplicatonLauncher>();
            launcher.LaunchEditor(file, lineNumber, 0);
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
