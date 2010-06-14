using System.IO;
using System.Threading;

using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging;

using Rhino.Mocks;
using NUnit.Framework;

namespace AutoTest.Test.Core
{
    [TestFixture]
    public class DirectoryWatcherTests
    {
        //this is not tagged as a fact as it actually modifies file system
        //it's an integration test (clieanup integration tests later)
        [Test]
        public void Should_send_message_when_file_changes_once()
        {
            var messageBus = MockRepository.GenerateMock<IMessageBus>();
            var path = Path.GetFullPath("asldkfjasdlfkjasdf.txt");
            using (var subject = new DirectoryWatcher(messageBus))
            {
                subject.Watch(Path.GetDirectoryName(path));
                // Write twice
                File.Create(path).Dispose();
                using (var writer = new StreamWriter(path, true)) { writer.WriteLine("some text"); }
                Thread.Sleep(50);
            }
            messageBus.AssertWasCalled(
                m => m.Publish<FileChangeMessage>(
                         Arg<FileChangeMessage>.Matches(
                             f => f.Extension.Equals(".txt") &&
                                  f.FullName.Equals(path) &&
                                  f.Name.Equals("asldkfjasdlfkjasdf.txt"))),
                m => m.Repeat.Once());
        }
    }
}