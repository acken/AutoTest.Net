using System;

namespace AutoTest.Core.Messaging
{
    using System.IO;

    public class FileChangeMessage : IMessage
    {
        public FileChangeMessage(string fullFilePath, string name, string extension)
        {
            _extension = extension;
            _fullName = fullFilePath;
            _name = name;
        }

        public FileChangeMessage(FileSystemInfo info)
        {
            if (info == null)
                throw new NullReferenceException("FileSystemInfo parameter is null");
            _extension = info.Extension;
            _fullName = info.FullName;
            _name = info.Name;
        }

        readonly string _extension;

        public string Extension
        {
            get { return _extension; }
        }

        readonly string _fullName;

        public string FullName
        {
            get { return _fullName; }
        }

        readonly string _name;

        public string Name
        {
            get { return _name; }
        }
    }
}