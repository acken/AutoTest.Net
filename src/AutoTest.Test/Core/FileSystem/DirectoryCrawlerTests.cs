using AutoTest.Core.FileSystem;
using NUnit.Framework;

namespace AutoTest.Test.Core
{
    [TestFixture]
    public class DirectoryCrawlerTests
    {
        [Test]
        public void Should_find_parent_of_windows_directory()
        {
            //start crawler in the location if this assembly
            var subject = new DirectoryCrawler();
            var parent = subject.FindParent("C:\\Windows\\System32", f => f.Directory.Name.Equals("C:\\"));
            parent.Directory.Name.ShouldEqual("C:\\");
        }

        [Test]
        public void Should_return_null_if_no_directory_found()
        {
            var subject = new DirectoryCrawler();
            var parent = subject.FindParent("C:\\", f => false);
            parent.ShouldBeNull();
        }
    }
}