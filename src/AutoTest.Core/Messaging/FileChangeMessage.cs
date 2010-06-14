using System;
using System.Collections.Generic;
using AutoTest.Core.FileSystem;

namespace AutoTest.Core.Messaging
{
    public class FileChangeMessage : IMessage
    {
        private List<ChangedFile> _files = new List<ChangedFile>();

        public ChangedFile[] Files { get { return _files.ToArray(); } }

        public void AddFile(ChangedFile file)
        {
            _files.Add(file);
        }

        public void AddFile(ChangedFile[] files)
        {
            _files.AddRange(files);
        }
    }
}