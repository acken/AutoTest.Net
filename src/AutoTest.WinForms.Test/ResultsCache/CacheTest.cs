using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.WinForms.ResultsCache;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;

namespace AutoTest.WinForms.Test.ResultsCache
{
    [TestFixture]
    public class CacheTest
    {
        private Cache _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new Cache();
        }

        [Test]
        public void Should_add_build_errors()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage());
            _cache.Merge(results);
            _cache.Errors.Length.ShouldEqual(1);
            _cache.Errors[0].Key.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_build_errors()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() {File = "some file", ErrorMessage = "some error message"});
            _cache.Merge(results);

            results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            results.AddError(new BuildMessage() { File = "some other file", ErrorMessage = "some other error message" });
            _cache.Merge(results);
            _cache.Errors.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_not_merge_same_build_errors_from_different_project()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _cache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _cache.Merge(results);
            _cache.Errors.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_build_errors_that_now_works()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _cache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _cache.Merge(results);

            results = new BuildRunResults("project");
            _cache.Merge(results);
            _cache.Errors.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_add_build_warnings()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage());
            _cache.Merge(results);
            _cache.Warnings.Length.ShouldEqual(1);
            _cache.Warnings[0].Key.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_build_warnings()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _cache.Merge(results);

            results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            results.AddWarning(new BuildMessage() { File = "some other file", ErrorMessage = "some other error message" });
            _cache.Merge(results);
            _cache.Warnings.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_not_merge_same_build_warnings_from_different_project()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _cache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _cache.Merge(results);
            _cache.Warnings.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_build_warnings_that_now_works()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _cache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _cache.Merge(results);

            results = new BuildRunResults("project");
            _cache.Merge(results);
            _cache.Warnings.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_add_failed_tests()
        {
            var results = new TestResult[]
                              {
                                  new TestResult(TestStatus.Failed, "Test name", "Message", new IStackLine[] {})
                              };
            var runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);
            _cache.Failed.Length.ShouldEqual(1);
            _cache.Failed[0].Key.ShouldEqual("assembly");
            _cache.Failed[0].Project.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_failed_tests()
        {
            var results = new TestResult[] { new TestResult(TestStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);

            _cache.Failed.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_not_merge_same_failed_tests_from_different_assemblies()
        {
            var results = new TestResult[] { new TestResult(TestStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "another assembly", results);
            _cache.Merge(runResults);

            _cache.Failed.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_failed_tests_that_now_passes()
        {
            var results = new TestResult[] { new TestResult(TestStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);
            
            runResults = new TestRunResults("project", "assembly", new TestResult[] {});
            _cache.Merge(runResults);

            _cache.Failed.Length.ShouldEqual(0);
        }

        [Test]
        public void Should_add_ignored_tests()
        {
            var results = new TestResult[]
                              {
                                  new TestResult(TestStatus.Ignored, "Test name", "Message", new IStackLine[] {})
                              };
            var runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);
            _cache.Ignored.Length.ShouldEqual(1);
            _cache.Ignored[0].Key.ShouldEqual("assembly");
            _cache.Ignored[0].Project.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_ignored_tests()
        {
            var results = new TestResult[] { new TestResult(TestStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);

            _cache.Ignored.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_not_merge_same_ignored_tests_from_different_assemblies()
        {
            var results = new TestResult[] { new TestResult(TestStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "another assembly", results);
            _cache.Merge(runResults);

            _cache.Ignored.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_ignored_tests_that_now_passes()
        {
            var results = new TestResult[]
                              {
                                  new TestResult(TestStatus.Ignored, "Test name", "Message", new IStackLine[] {}),
                                  new TestResult(TestStatus.Ignored, "Another test", "Message", new IStackLine[] {})
                              };
            var runResults = new TestRunResults("project", "assembly", results);
            _cache.Merge(runResults);

            runResults = new TestRunResults("project", "assembly", new TestResult[] { new TestResult(TestStatus.Ignored, "Another test", "Message", new IStackLine[] { }) });
            _cache.Merge(runResults);

            _cache.Ignored.Length.ShouldEqual(1);
        }
    }
}
