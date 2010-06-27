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
        private static DIContainer _container = new DIContainer();

        public static IServiceLocator Services { get { return _container.Services; } }
        public static IWindsorContainer Container { get { return _container.Services.Container; } }

        public static void Configure()
        {
            _container.Configure();
        }

        public static void InitializeCache()
        {
            _container.InitializeCache();
        }

        public static void RegisterAssembly(Assembly assembly)
        {
            _container.RegisterAssembly(assembly);
        }

        public static void ShutDown()
        {
            _container.Dispose();
            _container = new DIContainer();
        }
    }
}