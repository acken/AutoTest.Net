using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Configuration;
using Rhino.Mocks;
using AutoTest.Core.Messaging;
using AutoTest.Core.Caching.Projects;
using System.IO;

namespace AutoTest.Test.Core.Configuration
{
    [TestFixture]
    public class ConfigTest
    {
        private Config _config;
        private IMessageBus _bus;

        [SetUp]
        public void SetUp()
        {
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _config = new Config(_bus);
        }

        [Test]
        public void Should_read_directory_to_watch()
        {
            _config.WatchDirectores[0].ShouldEqual("TestResources");
        }

        [Test]
        public void Should_read_default_build_executable()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            _config.BuildExecutable(document).ShouldEqual(@"C:\Somefolder\MSBuild.exe");
        }

        [Test]
        public void Should_get_framework_spesific_build_executable()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("v3.5");
            _config.BuildExecutable(document).ShouldEqual(@"C:\SomefolderOther\MSBuild.exe");
        }

        [Test]
        public void Should_get_product_version_spesific_build_executable()
        {
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("v3.5");
            document.SetVSVersion("9.0.30729");
            _config.BuildExecutable(document).ShouldEqual(@"C:\ProductVersionFolder\MSBuild.exe");
        }

        [Test]
        public void Should_read_default_nunit_testrunner_path()
        {
            _config.NunitTestRunner("").ShouldEqual(@"C:\Somefolder\NUnit\nunit-console.exe");
        }

        [Test]
        public void Should_read_nunit_testrunner_path()
        {
            _config.NunitTestRunner("v3.5").ShouldEqual(@"C:\SomefolderOther\NUnit\nunit-console.exe");
        }

        [Test]
        public void Should_read_default_mstest_testrunner_path()
        {
            _config.MSTestRunner("").ShouldEqual(@"C:\Somefolder\MSTest.exe");
        }

        [Test]
        public void Should_read_mstest_testrunner_path()
        {
            _config.MSTestRunner("v3.5").ShouldEqual(@"C:\SomefolderOther\MSTest.exe");
        }

        [Test]
        public void Should_read_code_editor()
        {
            var editor = _config.CodeEditor;
            editor.Executable.ShouldEqual(@"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv");
            editor.Arguments.ShouldEqual("/Edit \"[[CodeFile]]\" /command \"Edit.Goto [[LineNumber]]\"");
        }

        [Test]
        public void Should_read_debug_flag()
        {
            var state = _config.DebuggingEnabled;
            state.ShouldBeTrue();
        }

        [Test]
        public void Should_read_default_xunit_testrunner_path()
        {
            _config.XunitTestRunner("").ShouldEqual(@"C:\Somefolder\XUnit\xunit.console.exe");
        }

        [Test]
        public void Should_read_xunit_testrunner_path()
        {
            _config.XunitTestRunner("v3.5").ShouldEqual(@"C:\SomefolderOther\XUnit\xunit.console.exe");
        }
		
		[Test]
		public void Should_read_growl_executable()
		{
			_config.GrowlNotify.ShouldEqual(@"C:\Meh\growlnotify.exe");
		}
		
		[Test]
		public void Should_read_notify_on_start()
		{
			_config.NotifyOnRunStarted.ShouldBeFalse();
		}
		
		[Test]
		public void Should_read_notify_on_completed()
		{
			_config.NotifyOnRunCompleted.ShouldBeFalse();
		}
		
		[Test]
		public void Should_get_watch_ignore_file()
		{
			var file = Path.Combine("TestResources", "myignorefile.txt");
			using (var writer = new StreamWriter(file))
			{
				writer.WriteLine("MyFolder");
				writer.WriteLine(@"OtherFolder\SomeFile.txt");
				writer.WriteLine(@"OhYeah\*");
				writer.WriteLine("*TestOutput.xml");
				writer.WriteLine("!meh.txt");
				writer.WriteLine("	#Comment");
				writer.WriteLine("");
			}
			
			_config.BuildIgnoreListFromPath("TestResources");
			_config.WatchIgnoreList.Length.ShouldEqual(4);
			_config.WatchIgnoreList[0].ShouldEqual("MyFolder");
			_config.WatchIgnoreList[1].ShouldEqual(@"OtherFolder\SomeFile.txt");
			_config.WatchIgnoreList[2].ShouldEqual(@"OhYeah\*");
			_config.WatchIgnoreList[3].ShouldEqual("*TestOutput.xml");
			if (File.Exists(file))
				File.Delete(file);
		}
		
		[Test]
		public void Should_set_to_empty_array_if_file_doesnt_exist()
		{
			_config.BuildIgnoreListFromPath("SomeInvalidDirectory");
			_config.WatchIgnoreList.Length.ShouldEqual(0);
		}
		
		[Test]
		public void Should_get_test_assemblies_to_ignore()
		{
			_config.TestAssembliesToIgnore[0].ShouldEqual("*System.dll");
			_config.TestAssembliesToIgnore[1].ShouldEqual("meh.exe");
		}
		
		[Test]
		public void Should_get_test_categories_to_ignore()
		{
			_config.TestCategoriesToIgnore[0].ShouldEqual("Category1");
			_config.TestCategoriesToIgnore[1].ShouldEqual("Category2");
		}
    }
}
