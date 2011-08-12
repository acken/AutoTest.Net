using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching;
using System.IO;

namespace AutoTest.Test.Core.Configuration
{
    [TestFixture]
    public class DIContainerFullTest
    {
        [Test]
        public void Should_perform_full_bootstrap()
        {
            var container = new DIContainer();
            container.Configure();
            container.InitializeCache(getWatchDirectory());
            var cache = container.Services.Locate<ICache>();
            cache.Count.ShouldEqual(4);
        }

        private string getWatchDirectory()
        {
            var path = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            return Path.Combine(path, "TestResources");
        }
    }
}
