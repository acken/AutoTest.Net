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
        [Test]
        public void Should_send_message_when_file_changes_once()
        {
            var file = "watcher_test.txt";
            var messageBus = MockRepository.GenerateMock<IMessageBus>();
            var path = Path.GetFullPath(file);
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
            File.Delete(file);
        }
    }
}