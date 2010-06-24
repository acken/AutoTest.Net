using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.Configuration;

namespace AutoTest.Test.Core.Configuration
{
    [TestFixture]
    public class ConfigTest
    {
        private Config _config;

        [SetUp]
        public void SetUp()
        {
            _config = new Config();
        }

        [Test]
        public void Should_read_directory_to_watch()
        {
            _config.DirectoryToWatch.ShouldEqual("TestResources\\");
        }

        [Test]
        public void Should_read_build_executable()
        {
            _config.BuildExecutable.ShouldEqual(@"C:\Somefolder\MSBuild.exe");
        }

        [Test]
        public void Should_read_nunit_testrunner_path()
        {
            _config.NunitTestRunner.ShouldEqual(@"C:\Somefolder\NUnit\nunit-console.exe");
        }

        [Test]
        public void Should_read_mstest_testrunner_path()
        {
            _config.MSTestRunner.ShouldEqual(@"C:\Somefolder\MSTest.exe");
        }

        [Test]
        public void Should_read_code_editor()
        {
            var editor = _config.CodeEditor;
            editor.Executable.ShouldEqual(@"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv");
            editor.Arguments.ShouldEqual("/Edit \"[[CodeFile]]\" /command \"Edit.Goto [[LineNumber]]\"");
        }
    }
}
