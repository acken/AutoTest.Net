using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.FileSystem.ProjectLocators;
using AutoTest.Core.FileSystem;
using AutoTest.Test.Core.FileSystem.Fakes;

namespace AutoTest.Test.Core.FileSystem.ProjectLocators
{
    [TestFixture]
    public class CSharpLocatorTest
    {
        [Test]
        public void Should_locate_csharp_project()
        {
            var changedProjects = new ChangedFile[] {};
            var fileLocator = new FakeProjectFileCrawler(changedProjects);
            var locator = new CSharpLocator(fileLocator);
            var files = locator.Locate("somechangedfile.cs");
            files.ShouldBeTheSameAs(changedProjects);
            fileLocator.ShouldHaveBeenAskedToLookFor(".csproj");
        }
    }
}
