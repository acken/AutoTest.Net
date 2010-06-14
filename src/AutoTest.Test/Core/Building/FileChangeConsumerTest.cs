using System;
using System.IO;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging;
using NUnit.Framework;
using Rhino.Mocks;
using AutoTest.Core.Configuration;
using AutoTest.Test.Core.Messaging.Fakes;
using AutoTest.Core.FileSystem.ProjectLocators;

namespace AutoTest.Test.Core.FileSystem
{
    [TestFixture]
    public class FileChangeConsumerTest
    {
        private IServiceLocator _services;
        private IMessageBus _bus;
        private FileChangeConsumer _subject;

        [SetUp]
        public void testSetup()
        {
            _services = MockRepository.GenerateMock<IServiceLocator>();
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _subject = new FileChangeConsumer(_services, _bus);
        }

        [Test]
        public void Should_not_publish_if_no_project_was_found()
        {
            var locator = new FakeProjectLocator(new ChangedFile[] { });
            _services.Stub(s => s.LocateAll<ILocateProjects>()).Return(new ILocateProjects[] { locator });
            var message = new FileChangeMessage();
            message.AddFile(new ChangedFile("somefile.cs"));
            _subject.Consume(message);
            _bus.AssertWasNotCalled(b => b.Publish<ProjectChangeMessage>(null), b => b.IgnoreArguments());
        }

        [Test]
        public void Should_publish_if_project_was_found()
        {
            var locator = new FakeProjectLocator(new ChangedFile[] { new ChangedFile("someproject.csproj") });
            _services.Stub(s => s.LocateAll<ILocateProjects>()).Return(new ILocateProjects[] { locator });
            var fileChange = new FileChangeMessage();
            fileChange.AddFile(new ChangedFile("asdf.cs"));
            _subject.Consume(fileChange);
            _bus.AssertWasCalled(b => b.Publish<ProjectChangeMessage>(null), b => b.IgnoreArguments());
        }
    }
}