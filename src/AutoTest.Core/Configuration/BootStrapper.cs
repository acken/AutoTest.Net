using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Caching;
using Castle.Core;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using AutoTest.Core.TestRunners;
using Castle.Windsor;
using AutoTest.Core.Caching.Crawlers;
using AutoTest.Core.FileSystem.ProjectLocators;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Presenters;

namespace AutoTest.Core.Configuration
{
    /// <summary>
    /// Bootstraps the AutoTest application...call this mother first
    /// </summary>
    public static class BootStrapper
    {
        private static ServiceLocator _services;

        public static IServiceLocator Services { get { return _services; } }
        public static IWindsorContainer Container { get { return _services.Container; } }

        public static void Configure()
        {
            _services = new ServiceLocator();
            _services.Container
                .Register(Component.For<IServiceLocator>().Instance(_services))
                .Register(Component.For<IMessageBus>().ImplementedBy<MessageBus>().LifeStyle.Singleton)
                .Register(Component.For<IFileSystemService>().ImplementedBy<FileSystemService>())
                .Register(Component.For<IProjectParser>().ImplementedBy<ProjectParser>())
                .Register(Component.For<ICreate<Project>>().ImplementedBy<ProjectFactory>())
                .Register(Component.For<IPrepare<Project>>().ImplementedBy<ProjectPreparer>())
                .Register(Component.For<IConsumerOf<ProjectChangeMessage>>().ImplementedBy<ProjectChangeConsumer>())
                .Register(Component.For<ICache>().ImplementedBy<Cache>().LifeStyle.Singleton)
                .Register(Component.For<IWatchValidator>().ImplementedBy<WatchValidator>())
                .Register(Component.For<ILocateProjects>().ImplementedBy<CSharpLocator>())
                .Register(Component.For<ILocateProjects>().ImplementedBy<VisualBasicLocator>())
                .Register(Component.For<IInformationFeedbackPresenter>().ImplementedBy<InformationFeedbackPresenter>())
                .Register(Component.For<IRunFeedbackPresenter>().ImplementedBy<RunFeedbackPresenter>());
            RegisterAssembly(Assembly.GetExecutingAssembly());
        }

        public static void InitializeCache()
        {
            var configuration = _services.Locate<IConfiguration>();
            var fsService = _services.Locate<IFileSystemService>();
            var cache = _services.Locate<ICache>();
            var cacheCrawler = new ProjectCrawler(cache, fsService);
            cacheCrawler.Crawl(configuration.DirectoryToWatch);
        }

        public static void RegisterAssembly(Assembly assembly)
        {
            _services.Container
                .Register(AllTypes
                              .Pick()
                              .FromAssembly(assembly)
                              .WithService
                              .FirstInterface());
        }
    }
}