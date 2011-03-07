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
    public class TypeLocatorTests : BaseClass
    {
        int _someField;

        [Test]
        public void Should_find_me()
        {
            var locator = new SimpleTypeLocator(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath, "AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests.Should_find_me");
            var method = locator.Locate();
            Assert.That(method.Category, Is.EqualTo(TypeCategory.Method));
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests.Should_find_me", method.Fullname);
            Assert.AreEqual("NUnit.Framework.TestAttribute", method.Attributes.ElementAt(0));
            Assert.AreEqual("System.Void", method.TypeName);
            Assert.AreEqual(0, method.Methods.Count());
            Assert.AreEqual(0, method.Fields.Count());
            Assert.AreEqual(true, method.IsPublic);
        }

        [Test]
        public void Should_find_my_parent()
        {
            var locator = new SimpleTypeLocator(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath, "AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests.Should_find_me");
            var cls = locator.LocateParent();
            Assert.That(cls.Category, Is.EqualTo(TypeCategory.Class));
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests", cls.Fullname);
            Assert.AreEqual("NUnit.Framework.TestFixtureAttribute", cls.Attributes.ElementAt(0));
            Assert.AreEqual(null, cls.TypeName);
            Assert.AreEqual(5, cls.Methods.Count());
            Assert.AreEqual(1, cls.Fields.Count());
            Assert.AreEqual("System.Int32", cls.Fields.First().TypeName);
            Assert.AreEqual(true, cls.IsPublic);
        }

        [Test]
        public void Should_find_inherited_attributes()
        {
            var locator = new SimpleTypeLocator(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath, "AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests");
            var cls = locator.Locate();
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.TypeLocatorTests", cls.Fullname);
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.MyAttribute", cls.Attributes.ElementAt(1));
        }

        [Test]
        public void Should_find_inherited_attributes_in_methods()
        {
            var locator = new SimpleTypeLocator(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath, "AutoTest.TestRunners.Tests.AssemblyAnalysis.BaseClass.Blargh");
            var cls = locator.Locate();
            Assert.AreEqual("AutoTest.TestRunners.Tests.AssemblyAnalysis.BaseClass.Blargh", cls.Fullname);
            Assert.AreEqual(3, cls.Attributes.Count());
        }
    }
    
    [MyAttribute]
    public class BaseClass
    {
        [MyOtherAttribute]
        public void Blargh()
        {
        }
    }

    public class MyAttribute : Attribute
    {
    }

    public class MyOtherAttribute : MyAttribute
    {
    }
}
