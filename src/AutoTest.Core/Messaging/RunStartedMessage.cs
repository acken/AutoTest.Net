using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem;

namespace AutoTest.Core.Messaging
{
    public class RunStartedMessage : IMessage
    {
        private ChangedFile[] _files;

        public ChangedFile[] Files { get { return _files; } }

        public RunStartedMessage(ChangedFile[] files)
        {
            _files = files;
        }
    }
}
