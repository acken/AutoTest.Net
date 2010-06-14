using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
using AutoTest.Test.Core.FileSystem.Fakes;
using AutoTest.Core.FileSystem.ProjectLocators;

namespace AutoTest.Test.Core.FileSystem.ProjectLocators
{
    [TestFixture]
    public class VisualBasicLocatorTest
    {
        [Test]
        public void Should_locate_visual_basic_project()
        {
            var changedProjects = new ChangedFile[] { };
            var fileLocator = new FakeProjectFileCrawler(changedProjects);
            var locator = new VisualBasicLocator(fileLocator);
            var files = locator.Locate("somechangedfile.vb");
            files.ShouldBeTheSameAs(changedProjects);
            fileLocator.ShouldHaveBeenAskedToLookFor(".vbproj");
        }
    }
}
