using System;
using System.IO;
using AutoTest.Core.Messaging;
using NUnit.Framework;

namespace AutoTest.Test.Core.Messaging
{
    [TestFixture]
    public class FileChangeMessageTests : MessageTestBase<FileChangeMessage>
    {
        protected override FileChangeMessage CreateMessage()
        {
            return new FileChangeMessage(new FileInfo("C:\\windows\\regedit.exe"));
        }

        [Test]
        public void Should_have_file_info() 
        { 
            var message = CreateMessage(); 
            message.FullName.ShouldEqual("C:\\windows\\regedit.exe");
            message.Extension.ShouldEqual(".exe");
            message.Name.ShouldEqual("regedit.exe");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SHould_not_allow_null_info()
        {
            new FileInfo(null);
        }

        [Test]
        public void Should_initialize_from_strings()
        {
            var message = new FileChangeMessage("1", "2", "3");
            message.FullName.ShouldEqual("1");
            message.Name.ShouldEqual("2");
            message.Extension.ShouldEqual("3");
        }
    }
}