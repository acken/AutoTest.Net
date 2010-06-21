using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;

namespace AutoTest.WinForms.ResultsCache
{
    class BuildItem
    {
        public string Key { get; private set; }
        public BuildMessage Value { get; private set; }

        public BuildItem(string key, BuildMessage value)
        {
            Key = key;
            Value = value;
        }

        public override bool  Equals(object obj)
        {
            var other = (BuildItem) obj;
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}", Key, Value.GetHashCode().ToString()).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(
                "Project: {0}\r\n" +
                "File: {1}:line {2}\r\n" +
                "Message:\r\n{3}",
                Key,
                Value.File,
                Value.LineNumber,
                Value.ErrorMessage);
        }
    }
}
