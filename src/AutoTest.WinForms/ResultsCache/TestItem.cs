using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;

namespace AutoTest.WinForms.ResultsCache
{
    class TestItem
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
            return string.Format(
                "Assembly: {0}\r\n" +
                "Test: {1}\r\n" +
                "Message:\r\n{2}\r\n" +
                "Stack trace\r\n{3}",
                Key,
                Value.Name,
                Value.Message,
                Value.StackTrace);
        }
    }
}
