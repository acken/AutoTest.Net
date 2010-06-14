using AutoTest.Core.Configuration;
using AutoTest.Core.Messaging;
using NUnit.Framework;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Caching;
using AutoTest.Core.TestRunners;
using System;
using AutoTest.Core.FileSystem;

namespace AutoTest.Test.Core.Configuration
{
    [TestFixture]
    public class BootStrapperTests
    {
        private IServiceLocator _locator;

        public BootStrapperTests()
        {
            BootStrapper.Configure();
        }

        [SetUp]
        public void SetUp()
        {
            _locator = BootStrapper.Services;
        }

        [Test]
        public void Should_be_able_to_get_service_locator_from_container()
        {
            _locator.Locate<IServiceLocator>().ShouldBeOfType<IServiceLocator>();
        }

        [Test]
        public void Should_initialize_setting_service()
        {
            _locator.Locate<IConfiguration>().ShouldBeOfType<IConfiguration>();
        }


        [Test]
        public void Should_register_messaging_module()
        {
            var bus = _locator.Locate<IMessageBus>();
            bus.ShouldBeOfType<IMessageBus>();
        }

        [Test]
        public void Should_register_project_parser()
        {
            var parser = _locator.Locate<IProjectParser>();
            parser.ShouldBeOfType<IProjectParser>();
        }

        [Test]
        public void Should_register_project_factory()
        {
            var factory = _locator.Locate<ICreate<Project>>();
            factory.ShouldBeOfType<ICreate<Project>>();
        }

        [Test]
        public void Should_register_project_preparer()
        {
            var preparer = _locator.Locate<IPrepare<Project>>();
            var preparer2 = _locator.Locate<IPrepare<Project>>();
            preparer.ShouldBeOfType<IPrepare<Project>>();
            preparer.ShouldBeTheSameAs(preparer2);
        }

        [Test]
        public void Should_register_cache()
        {
            var cache = _locator.Locate<ICache>();
            cache.ShouldBeOfType<ICache>();
        }

        [Test]
        public void Should_register_cache_as_singleton()
        {
            var cache = _locator.Locate<ICache>();
            var cache2 = _locator.Locate<ICache>();
            cache.ShouldBeTheSameAs(cache2);
        }

        [Test]
        public void Should_register_build_locator()
        {
            var buildLocator = _locator.Locate<IConsumerOf<ProjectChangeMessage>>();
            buildLocator.ShouldBeOfType<IConsumerOf<ProjectChangeMessage>>();
        }

        [Test]
        public void Should_bind_consumer_of_file_change_message()
        {
            int i = 0;
            var filChangeHandlers = _locator.LocateAll<IConsumerOf<FileChangeMessage>>();
            foreach (var change in filChangeHandlers)
                i++;
            i.ShouldEqual(2);
        }

        [Test]
        public void Should_register_test_runner()
        {
            var testRunner = _locator.Locate<ITestRunner>();
            testRunner.ShouldBeOfType<CommandLineTestRunner>();
        }

        [Test]
        public void Should_register_file_system_service()
        {
            var service = _locator.Locate<IFileSystemService>();
            service.ShouldBeOfType<IFileSystemService>();
        }

        [Test]
        public void Should_register_watch_validator()
        {
            var validator = _locator.Locate<IWatchValidator>();
            validator.ShouldBeOfType<IWatchValidator>();
        }
    }
}