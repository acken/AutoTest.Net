using System;
using System.IO;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging;
using NUnit.Framework;
using Rhino.Mocks;

namespace AutoTest.Test.Core.Building
{
    [TestFixture]
    public class CSharpProjectLocatorTests
    {
        private IMessageBus _bus;
        private IDirectoryCrawler _crawler;
        private CSharpProjectLocator _subject;

        [SetUp]
        public void testSetup()
        {
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _crawler = MockRepository.GenerateMock<IDirectoryCrawler>();
            _subject = new CSharpProjectLocator(_bus, _crawler);
        }

        [Test]
        public void Should_not_publish_on_non_project_event()
        {
            _subject.Consume(new FileChangeMessage("1", "2", "3"));
        }

        [Test]
        public void Should_not_publish_if_no_project_was_found()
        {
            _subject.Consume(new FileChangeMessage("C:\\windows\\regedit.exe", "asdf", ".cs"));
            _crawler.AssertWasCalled(c => c.FindParent(null, null), c => c.IgnoreArguments());
        }

        [Test]
        public void Should_publish_if_project_was_found()
        {
            _crawler.Stub(c => c.FindParent("", f => true))
                .IgnoreArguments()
                .Return(new FileInfo("C:\\windows\\regedit.exe"));
            _subject.Consume(new FileChangeMessage("asdf", "asdf", ".cs"));
            _crawler.AssertWasCalled(c => c.FindParent("", null), c => c.IgnoreArguments());
            _bus.AssertWasCalled(b => b.Publish<ProjectChangeMessage>(null), b => b.IgnoreArguments());
        }
    }
}