using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Messages;

namespace AutoTest.Test.Core.Caching
{
    [TestFixture]
    public class RunResultCacheTest
    {
        private RunResultCache _runResultCache;

        [SetUp]
        public void SetUp()
        {
            _runResultCache = new RunResultCache();
        }

        [Test]
        public void Should_add_build_errors()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage());
            _runResultCache.Merge(results);
            _runResultCache.Errors.Length.ShouldEqual(1);
            _runResultCache.Errors[0].Key.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_build_errors()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() {File = "some file", ErrorMessage = "some error message"});
            _runResultCache.Merge(results);

            results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            results.AddError(new BuildMessage() { File = "some other file", ErrorMessage = "some other error message" });
            _runResultCache.Merge(results);
            _runResultCache.Errors.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_not_merge_same_build_errors_from_different_project()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);
            _runResultCache.Errors.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_build_errors_that_now_works()
        {
            var results = new BuildRunResults("project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddError(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("project");
            _runResultCache.Merge(results);
            _runResultCache.Errors.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_add_build_warnings()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage());
            _runResultCache.Merge(results);
            _runResultCache.Warnings.Length.ShouldEqual(1);
            _runResultCache.Warnings[0].Key.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_build_warnings()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            results.AddWarning(new BuildMessage() { File = "some other file", ErrorMessage = "some other error message" });
            _runResultCache.Merge(results);
            _runResultCache.Warnings.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_not_merge_same_build_warnings_from_different_project()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);
            _runResultCache.Warnings.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_build_warnings_that_now_works()
        {
            var results = new BuildRunResults("project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("another project");
            results.AddWarning(new BuildMessage() { File = "some file", ErrorMessage = "some error message" });
            _runResultCache.Merge(results);

            results = new BuildRunResults("project");
            _runResultCache.Merge(results);
            _runResultCache.Warnings.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_add_failed_tests()
        {
            var results = new TestResult[]
                              {
                                  new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] {})
                              };
            var runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);
            _runResultCache.Failed.Length.ShouldEqual(1);
            _runResultCache.Failed[0].Key.ShouldEqual("assembly");
            _runResultCache.Failed[0].Project.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_failed_tests()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Failed.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_not_merge_same_failed_tests_from_different_assemblies()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "another assembly", false, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Failed.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_not_merge_same_failed_tests_with_different_runners()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.MSTest, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Failed.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_failed_tests_that_now_passes()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Failed.Length.ShouldEqual(0);
        }

        [Test]
        public void Should_add_ignored_tests()
        {
            var results = new TestResult[]
                              {
                                  new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] {})
                              };
            var runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);
            _runResultCache.Ignored.Length.ShouldEqual(1);
            _runResultCache.Ignored[0].Key.ShouldEqual("assembly");
            _runResultCache.Ignored[0].Project.ShouldEqual("project");
        }

        [Test]
        public void Should_merge_ignored_tests()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_not_merge_same_ignored_tests_from_different_assemblies()
        {
            var results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            var runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            results = new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) };
            runResults = new TestRunResults("project", "another assembly", false, results);
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(2);
        }

        [Test]
        public void Should_remove_cached_ignored_tests_that_now_passes()
        {
            var results = new TestResult[]
                              {
                                  new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] {}),
                                  new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Another test", "Message", new IStackLine[] {})
                              };
            var runResults = new TestRunResults("project", "assembly", false, results);
            _runResultCache.Merge(runResults);

            runResults = new TestRunResults("project", "assembly", false, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Passed, "Test name", "Message", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(1);
        }

        [Test]
        public void Should_merge_tests_going_from_failed_to_ignored()
        {
            var runResults = new TestRunResults("project", "assembly", false, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            runResults = new TestRunResults("project", "assembly", true, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] {}) });
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(1);
            _runResultCache.Failed.Length.ShouldEqual(0);
        }

        [Test]
        public void Should_merge_tests_going_from_ignored_to_failed()
        {
            var runResults = new TestRunResults("project", "assembly", false, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Ignored, "Test name", "Message", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            runResults = new TestRunResults("project", "assembly", true, new TestResult[] { new TestResult(TestRunner.NUnit, TestRunStatus.Failed, "Test name", "Message", new IStackLine[] { }) });
            _runResultCache.Merge(runResults);

            _runResultCache.Ignored.Length.ShouldEqual(0);
            _runResultCache.Failed.Length.ShouldEqual(1);
        }
    }
}
