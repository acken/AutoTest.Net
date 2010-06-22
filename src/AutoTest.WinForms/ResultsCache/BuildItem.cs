using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using System.IO;

namespace AutoTest.WinForms.ResultsCache
{
    class BuildItem : IItem
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
                "File: {4}{1}:line {2}{5}\r\n" +
                "Message:\r\n{3}",
                Key,
                Value.File,
                Value.LineNumber,
                Value.ErrorMessage,
                LinkParser.TAG_START,
                LinkParser.TAG_END);
        }

        #region IItem Members


        public void HandleLink(string link)
        {
            var file = Path.Combine(Path.GetDirectoryName(Key), Value.File);
            var launcher = new ApplicatonLauncher(file, Value.LineNumber);
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
