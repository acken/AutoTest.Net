using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Configuration;
using AutoTest.Core.Caching;

namespace AutoTest.Test.Core.Configuration
{
    [TestFixture]
    public class BootStrapperFullTest
    {
        [Test]
        public void Should_perform_full_bootstrap()
        {
            BootStrapper.Configure();
            BootStrapper.InitializeCache();
            var cache = BootStrapper.Services.Locate<ICache>();
            cache.Count.ShouldEqual(4);
        }
    }
}
