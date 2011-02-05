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
using AutoTest.Core.DebugLog;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using AutoTest.Core.TestRunners.TestRunners;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Core.Notifiers;
using AutoTest.Messages;
using System.IO;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.Core.Configuration
{
    public class DIContainer : IDisposable
    {
        private ServiceLocator _services;

        public IServiceLocator Services { get { return _services; } }
		public IWindsorContainer Container { get { return _services.Container; } }

		public void Configure()
		{
			Configure(null);
		}
		
        public void Configure(ILocateWriteLocation defaultConfigurationLocator)
        {
            _services = new ServiceLocator();
            _services.Container.Kernel.Resolver.AddSubResolver(new ArrayResolver(_services.Container.Kernel));
            _services.Container
                .Register(Component.For<IServiceLocator>().Instance(_services))
                .Register(Component.For<IMessageBus>().ImplementedBy<MessageBus>().LifeStyle.Singleton)
                .Register(Component.For<IFileSystemService>().ImplementedBy<FileSystemService>())
                .Register(Component.For<IProjectParser>().ImplementedBy<ProjectParser>())
                .Register(Component.For<ICreate<Project>>().ImplementedBy<ProjectFactory>())
                .Register(Component.For<IPrepare<Project>>().ImplementedBy<ProjectPreparer>())
                .Register(Component.For<IBlockingConsumerOf<ProjectChangeMessage>>().ImplementedBy<ProjectChangeConsumer>())
                .Register(Component.For<IConsumerOf<FileChangeMessage>>().ImplementedBy<FileChangeConsumer>().Named("MSBuild"))
				.Register(Component.For<IConsumerOf<FileChangeMessage>>().ImplementedBy<BinaryFileChangeConsumer>().Named("NoBuild"))
                .Register(Component.For<ICache>().ImplementedBy<Cache>().LifeStyle.Singleton)
                .Register(Component.For<IWatchValidator>().ImplementedBy<WatchValidator>())
                .Register(Component.For<ILocateProjects>().ImplementedBy<CSharpLocator>())
                .Register(Component.For<ILocateProjects>().ImplementedBy<VisualBasicLocator>())
                .Register(Component.For<IInformationFeedbackPresenter>().ImplementedBy<InformationFeedbackPresenter>())
                .Register(Component.For<IRunFeedbackPresenter>().ImplementedBy<RunFeedbackPresenter>())
                .Register(Component.For<IDirectoryWatcher>().ImplementedBy<DirectoryWatcher>().LifeStyle.Singleton)
                .Register(Component.For<IConfiguration>().ImplementedBy<Config>())
                .Register(Component.For<ICrawlForProjectFiles>().ImplementedBy<ProjectFileCrawler>())
                .Register(Component.For<IReload<Project>>().ImplementedBy<ProjectReloader>())
                .Register(Component.For<IPrioritizeProjects>().ImplementedBy<ProjectPrioritizer>())
                .Register(Component.For<IBuildRunner>().ImplementedBy<MSBuildRunner>())
                .Register(Component.For<ITestRunner>().ImplementedBy<NUnitTestRunner>())
                .Register(Component.For<ITestRunner>().ImplementedBy<MSTestRunner>())
                .Register(Component.For<ITestRunner>().ImplementedBy<XUnitTestRunner>())
                .Register(Component.For<ITestRunner>().ImplementedBy<MSpecTestRunner>())
                .Register(Component.For<ITestRunner>().ImplementedBy<AutoTestTestRunner>())
                .Register(Component.For<IExternalProcess>().ImplementedBy<HiddenExternalProcess>())
                .Register(Component.For<IMSpecCommandLineBuilder>().ImplementedBy<MSpecCommandLineBuilder>())
                .Register(Component.For<IGenerateBuildList>().ImplementedBy<BuildListGenerator>())
                .Register(Component.For<IMergeRunResults>().Forward<IRunResultCache>().ImplementedBy<RunResultCache>())
				.Register(Component.For<IRetrieveAssemblyIdentifiers>().ImplementedBy<AssemblyParser>())
				.Register(Component.For<IConsumerOf<AssemblyChangeMessage>>().ImplementedBy<AssemblyChangeConsumer>())
				.Register(Component.For<IDetermineIfAssemblyShouldBeTested>().ImplementedBy<TestRunValidator>())
				.Register(Component.For<IOptimizeBuildConfiguration>().ImplementedBy<BuildOptimizer>())
				.Register(Component.For<IPreProcessTestruns>().ImplementedBy<NullPreProcessor>())
                .Register(Component.For<IPreProcessBuildruns>().ImplementedBy<NullBuildPreProcessor>())
				.Register(Component.For<IHandleDelayedConfiguration>().ImplementedBy<DelayedConfigurer>())
				.Register(Component.For<IMarkProjectsForRebuild>().ImplementedBy<ProjectRebuildMarker>())
                .Register(Component.For<ILocateRemovedTests>().ImplementedBy<RemovedTestsLocator>())
                .Register(Component.For<ISolutionChangeConsumer>().ImplementedBy<SolutionChangeConsumer>())
                .Register(Component.For<ISolutionParser>().ImplementedBy<SolutionCrawler>())
                .Register(Component.For<IAssemblyReader>().ImplementedBy<AssemblyReader>())
                .Register(Component.For<ApplicatonLauncher>())
                .Register(Component.For<IWriteDebugInfo>().ImplementedBy<DebugWriter>().LifeStyle.Singleton);

            if (defaultConfigurationLocator == null)
                defaultConfigurationLocator = new DefaultConfigurationLocator();
            _services.Container.Register(Component.For<ILocateWriteLocation>().Instance(defaultConfigurationLocator));
			
			initializeNotifiers();
        }
		
		public void AddRunFailedTestsFirstPreProcessor()
		{
			_services.Container.Register(Component.For<IPreProcessTestruns>().ImplementedBy<RunFailedTestsFirstPreProcessor>());
		}

        public void InitializeCache(string watchFolder)
        {
            setWatchFolder(watchFolder);
            if (isFolder(watchFolder))
                crawlFolder(watchFolder);
            else
                crawlFile(watchFolder);
        }

        private void setWatchFolder(string watchFolder)
        {
            var config = _services.Locate<IConfiguration>();
            config.SetWatchPath(watchFolder);
        }

        private void crawlFile(string solutionFile)
        {
            var fsService = _services.Locate<IFileSystemService>();
            var cache = _services.Locate<ICache>();
            var bus = _services.Locate<IMessageBus>();
            var crawler = new SolutionCrawler(fsService, bus, cache);
            crawler.Crawl(solutionFile);
        }

        private void crawlFolder(string watchFolder)
        {
            var fsService = _services.Locate<IFileSystemService>();
            var cache = _services.Locate<ICache>();
            var bus = _services.Locate<IMessageBus>();
            var cacheCrawler = new ProjectCrawler(cache, fsService, bus);
            cacheCrawler.Crawl(watchFolder);
        }

        private bool isFolder(string watchFolder)
        {
            return Directory.Exists(watchFolder);
        }

        public void RegisterAssembly(Assembly assembly)
        {
            _services.Container
                .Register(AllTypes
                              .FromAssembly(assembly)
				              .Pick()
                              .WithService
                              .FirstInterface());
        }

        #region IDisposable Members

        public void Dispose()
        {
            _services.UnregisterAll();
        }

        #endregion
		
		private void initializeNotifiers()
		{
            if ((new notify_sendNotifier()).IsSupported())
                _services.Container.Register(Component.For<ISendNotifications>().ImplementedBy<notify_sendNotifier>());
            else if ((new SnarlNotifier()).IsSupported())
                _services.Container.Register(Component.For<ISendNotifications>().ImplementedBy<SnarlNotifier>());
            else if ((new GrowlNotifier(null)).IsSupported())
                _services.Container.Register(Component.For<ISendNotifications>().ImplementedBy<GrowlNotifier>());
            else
                _services.Container.Register(Component.For<ISendNotifications>().ImplementedBy<NullNotifier>());
		}
    }
}
