using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;
using System.IO;
using AutoTest.Core.Launchers;
using AutoTest.Core.Configuration;

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
            if (File.Exists(getFilePath()))
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
            else
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

        #region IItem Members


        public void HandleLink(string link)
        {
            var file = getFilePath();
            var launcher = BootStrapper.Services.Locate<ApplicatonLauncher>();
            launcher.LaunchEditor(file, Value.LineNumber);
        }

        private string getFilePath()
        {
            return Path.Combine(Path.GetDirectoryName(Key), Value.File);
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
