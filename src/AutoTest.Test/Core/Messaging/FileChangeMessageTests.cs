using System;
using AutoTest.Core.Messaging;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
using System.IO;

namespace AutoTest.Test.Core.Messaging
{
    [TestFixture]
    public class FileChangeMessageTests : MessageTestBase<FileChangeMessage>
    {
        protected override FileChangeMessage CreateMessage()
        {
            var fileChange = new FileChangeMessage();
            fileChange.AddFile(new ChangedFile("C:\\windows\\regedit.exe"));
            return fileChange;
        }

        [Test]
        public void Should_have_file_info() 
        { 
            var message = CreateMessage(); 
            message.Files[0].FullName.ShouldEqual("C:\\windows\\regedit.exe");
            message.Files[0].Extension.ShouldEqual(".exe");
            message.Files[0].Name.ShouldEqual("regedit.exe");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SHould_not_allow_null_info()
        {
            new ChangedFile(null);
        }

        [Test]
        public void Should_initialize_from_strings()
        {
            var message = new FileChangeMessage();
            message.AddFile(new ChangedFile(Path.Combine("2", "1.1")));
            message.Files[0].FullName.ShouldEqual(Path.Combine("2", "1.1"));
            message.Files[0].Name.ShouldEqual("1.1");
            message.Files[0].Extension.ShouldEqual(".1");
        }
    }
}