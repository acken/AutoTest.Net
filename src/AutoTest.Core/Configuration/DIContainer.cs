using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Caching;
using AutoTest.Core.FileSystem.ProjectLocators;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging.MessageConsumers;
using System.Reflection;
using AutoTest.Core.Caching.Crawlers;
using AutoTest.Core.Launchers;

namespace AutoTest.Core.Configuration
{
    class DIContainer : IDisposable
    {
        private ServiceLocator _services;

        public ServiceLocator Services { get { return _services; } }

        public void Configure()
        {
            _services = new ServiceLocator();
            _services.Container
                .Register(Component.For<IServiceLocator>().Instance(_services))
                .Register(Component.For<IMessageBus>().ImplementedBy<MessageBus>().LifeStyle.Singleton)
                .Register(Component.For<IFileSystemService>().ImplementedBy<FileSystemService>())
                .Register(Component.For<IProjectParser>().ImplementedBy<ProjectParser>())
                .Register(Component.For<ICreate<Project>>().ImplementedBy<ProjectFactory>())
                .Register(Component.For<IPrepare<Project>>().ImplementedBy<ProjectPreparer>())
                .Register(Component.For<IBlockingConsumerOf<ProjectChangeMessage>>().ImplementedBy<ProjectChangeConsumer>())
                .Register(Component.For<IConsumerOf<FileChangeMessage>>().ImplementedBy<FileChangeConsumer>())
                .Register(Component.For<ICache>().ImplementedBy<Cache>().LifeStyle.Singleton)
                .Register(Component.For<IWatchValidator>().ImplementedBy<WatchValidator>())
                .Register(Component.For<ILocateProjects>().ImplementedBy<CSharpLocator>())
                .Register(Component.For<ILocateProjects>().ImplementedBy<VisualBasicLocator>())
                .Register(Component.For<IInformationFeedbackPresenter>().ImplementedBy<InformationFeedbackPresenter>())
                .Register(Component.For<IRunFeedbackPresenter>().ImplementedBy<RunFeedbackPresenter>())
                .Register(Component.For<IDirectoryWatcher>().ImplementedBy<DirectoryWatcher>())
                .Register(Component.For<IConfiguration>().ImplementedBy<Config>())
                .Register(Component.For<ICrawlForProjectFiles>().ImplementedBy<ProjectFileCrawler>())
                .Register(Component.For<ApplicatonLauncher>());
        }

        public void InitializeCache()
        {
            var configuration = _services.Locate<IConfiguration>();
            var fsService = _services.Locate<IFileSystemService>();
            var cache = _services.Locate<ICache>();
            var cacheCrawler = new ProjectCrawler(cache, fsService);
            cacheCrawler.Crawl(configuration.DirectoryToWatch);
        }

        public void RegisterAssembly(Assembly assembly)
        {
            _services.Container
                .Register(AllTypes
                              .Pick()
                              .FromAssembly(assembly)
                              .WithService
                              .FirstInterface());
        }

        #region IDisposable Members

        public void Dispose()
        {
            _services.UnregisterAll();
        }

        #endregion
    }
}
