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
using AutoTest.Core.DebugLog;

namespace AutoTest.Core.Configuration
{
    /// <summary>
    /// Bootstraps the AutoTest application...call this mother first
    /// </summary>
    public static class BootStrapper
    {
        private static DIContainer _container = new DIContainer();

        public static IServiceLocator Services { get { return _container.Services; } }
        public static IWindsorContainer Container { get { return _container.Container; } }
		public static DIContainer DIContainer { get { return _container; } }

        public static void Configure()
        {
            _container.Configure();
			var configuration = _container.Services.Locate<IConfiguration>();
            if (configuration.DebuggingEnabled)
                Debug.EnableLogging();
			Debug.InitialConfigurationFinished();
        }

        public static void InitializeCache(string watchFolder)
        {
            _container.InitializeCache(watchFolder);
            Debug.InitializedCache();
        }

        public static void RegisterAssembly(Assembly assembly)
        {
            _container.RegisterAssembly(assembly);
            Debug.RegisteredAssembly(assembly);
        }

        public static void ShutDown()
        {
            Debug.ShutingDownContainer();
            _container.Dispose();
            _container = new DIContainer();
        }
    }
}