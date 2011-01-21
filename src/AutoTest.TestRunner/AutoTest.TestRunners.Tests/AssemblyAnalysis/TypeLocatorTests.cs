using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;
using System.IO;
using System.Reflection;

namespace AutoTest.TestRunners.Tests.AssemblyAnalysis
{
    [TestFixture]
    public class TypeLocatorTests
    {
        [Test]
        public void Should_find_me()
        {
            var locator = new SimpleTypeLocator(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath, "AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests.Should_find_me");
            var method = locator.Locate();
            Assert.That(method.Category, Is.EqualTo(TypeCategory.Method));
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests.Should_find_me", method.Fullname);
            Assert.AreEqual("NUnit.Framework.TestAttribute", method.Attributes.ElementAt(0));
            Assert.AreEqual(0, method.Methods.Count());
        }

        [Test]
        public void Should_find_my_parent()
        {
            var locator = new SimpleTypeLocator(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath, "AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests.Should_find_me");
            var cls = locator.LocateParent();
            Assert.That(cls.Category, Is.EqualTo(TypeCategory.Class));
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests", cls.Fullname);
            Assert.AreEqual("NUnit.Framework.TestFixtureAttribute", cls.Attributes.ElementAt(0));
            Assert.AreEqual(3, cls.Methods.Count());
        }
    }
}
