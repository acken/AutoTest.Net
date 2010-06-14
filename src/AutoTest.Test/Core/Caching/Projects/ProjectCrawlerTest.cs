using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem;
using NUnit.Framework;
using System.IO;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Caching.Crawlers;
using AutoTest.Test.Core.Caching.Projects.Fakes;

namespace AutoTest.Test.Core.Caching.Projects
{
    [TestFixture]
    public class ProjectCrawlerTest
    {
        private ProjectCrawler _crawler;
        private FakeCache _cache;
        private FakeFileSystemService _fsService;

        [SetUp]
        public void setUp()
        {
            _fsService = new FakeFileSystemService();
            _cache = new FakeCache();
            _crawler = new ProjectCrawler(_cache, _fsService);
        }

        [Test]
        public void Should_Find_CSharp_Projects()
        {
            _fsService.WhenCrawlingFor("*.csproj").Return("AProject.csproj");
            _crawler.Crawl("");
            _cache.ShouldHaveBeenAdded(Path.GetFullPath("AProject.csproj"));
        }

        [Test]
        public void Should_Find_VisualBasic_Projects()
        {
            _fsService.WhenCrawlingFor("*.vbproj").Return("AProject.vbproj");
            _crawler.Crawl("");
            _cache.ShouldHaveBeenAdded(Path.GetFullPath("AProject.vbproj"));
        }
    }
}
