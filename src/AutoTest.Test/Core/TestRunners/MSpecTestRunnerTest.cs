using System;

using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Configuration;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.TestRunners.TestRunners;
using AutoTest.Messages;

using NUnit.Framework;

using Rhino.Mocks;
using System.IO;

namespace AutoTest.Test.Core.TestRunners
{
    [TestFixture]
    public class MSpecTestRunnerTest
    {
        [SetUp]
        public void SetUp()
        {
            _configuration = MockRepository.GenerateMock<IConfiguration>();
            _referenceResolver = MockRepository.GenerateMock<IResolveAssemblyReferences>();
            _fileSystem = MockRepository.GenerateStub<IFileSystemService>();
            _externalProcess = MockRepository.GenerateStub<IExternalProcess>();
            _commandLineBuilder = MockRepository.GenerateStub<IMSpecCommandLineBuilder>();

            _runner = new MSpecTestRunner(_referenceResolver,
                                          _configuration,
                                          _fileSystem,
                                          _externalProcess,
                                          _commandLineBuilder);
        }

        MSpecTestRunner _runner;
        IConfiguration _configuration;
        IResolveAssemblyReferences _referenceResolver;
        IFileSystemService _fileSystem;
        IExternalProcess _externalProcess;
        IMSpecCommandLineBuilder _commandLineBuilder;

        [Test]
        public void Should_build_the_command_line_for_each_run()
        {
            _configuration
                .Stub(x => x.MSpecTestRunner("framework 1"))
                .Return("c:\\runner 1.exe");

            _configuration
                .Stub(x => x.MSpecTestRunner("framework 2"))
                .Return("c:\\runner 2.exe");

            _fileSystem
                .Stub(x => x.FileExists(null))
                .IgnoreArguments()
                .Return(true);

            var document1 = new ProjectDocument(ProjectType.CSharp);
            document1.SetFramework("framework 1");
            var info1 = new TestRunInfo(new Project("key 1", document1), "assembly 1");

            var document2 = new ProjectDocument(ProjectType.CSharp);
            document2.SetFramework("framework 2");
            var info2 = new TestRunInfo(new Project("key 2", document2), "assembly 2");

            var testRunInfos = new[] { info1, info2 };

            _runner.RunTests(testRunInfos);

            _commandLineBuilder.AssertWasCalled(x => x.Build(null),
                                                o => o.IgnoreArguments().Repeat.Twice());
        }

        [Test]
        public void Should_check_for_mspec_test_framework_reference()
        {
            var assembly = String.Empty;
            _referenceResolver.Stub(r => r.GetReferences(assembly)).Return(new[] { "machine.specifications" });

            var handles = _runner.CanHandleTestFor(assembly);

            handles.ShouldBeTrue();
        }

        [Test]
        public void Should_check_the_runner_exe_for_each_framework()
        {
            _configuration
                .Stub(x => x.MSpecTestRunner("framework 1"))
                .Return("c:\\runner 1.exe");

            _configuration
                .Stub(x => x.MSpecTestRunner("framework 2"))
                .Return("c:\\runner 2.exe");

            var document1 = new ProjectDocument(ProjectType.CSharp);
            document1.SetFramework("framework 1");
            var info1 = new TestRunInfo(new Project("key 1", document1), "assembly 1");

            var document2 = new ProjectDocument(ProjectType.CSharp);
            document2.SetFramework("framework 2");
            var info2 = new TestRunInfo(new Project("key 2", document2), "assembly 2");

            var testRunInfos = new[] { info1, info2 };

            _runner.RunTests(testRunInfos);

            _fileSystem.AssertWasCalled(x => x.FileExists("c:\\runner 1.exe"));
            _fileSystem.AssertWasCalled(x => x.FileExists("c:\\runner 2.exe"));
        }

        [Test]
        public void Should_handle_projects_referencing_mspec()
        {
            var projectFile = string.Format("TestResources{0}VS2008{0}CSharpNUnitTestProject.csproj", Path.DirectorySeparatorChar);
            _configuration.Stub(c => c.MSpecTestRunner("3.5")).Return("testRunner.exe");
            _fileSystem.Stub(x => x.FileExists("testRunner.exe")).Return(true);
            var document = new ProjectDocument(ProjectType.CSharp);
            document.SetFramework("3.5");

            var handles = _runner.CanHandleTestFor(new Project(projectFile, document));

            handles.ShouldBeTrue();
        }

        [Test]
        public void Should_not_handle_projects_not_referencing_mspec()
        {
            var document = new ProjectDocument(ProjectType.CSharp);

            var handles = _runner.CanHandleTestFor(new Project("someProject", document));

            handles.ShouldBeFalse();
        }

        [Test]
        public void Should_run_tests_for_each_framework_with_an_existing_runner()
        {
            _configuration
                .Stub(x => x.MSpecTestRunner("framework 1"))
                .Return("c:\\runner 1.exe");

            _configuration
                .Stub(x => x.MSpecTestRunner("framework 2"))
                .Return("c:\\runner 2.exe");

            _fileSystem
                .Stub(x => x.FileExists("c:\\runner 2.exe"))
                .Return(true);

            var document1 = new ProjectDocument(ProjectType.CSharp);
            document1.SetFramework("framework 1");
            var info1 = new TestRunInfo(new Project("key 1", document1), "assembly 1");

            var document2 = new ProjectDocument(ProjectType.CSharp);
            document2.SetFramework("framework 2");
            var info2 = new TestRunInfo(new Project("key 2", document2), "assembly 2");
            info2.AddTestsToRun(new[]
                                {
                                    new TestToRun(TestRunner.MSpec, "test 1"),
                                });

            var testRunInfos = new[] { info1, info2 };

            _runner.RunTests(testRunInfos);

            _externalProcess.AssertWasNotCalled(
                x => x.CreateAndWaitForExit(Arg<string>.Matches(y => y == "c:\\runner 1.exe"),
                                                   Arg<string>.Is.Anything));

            _externalProcess.AssertWasCalled(
                x => x.CreateAndWaitForExit(Arg<string>.Matches(y => y == "c:\\runner 2.exe"),
                                                   Arg<string>.Is.Anything));
        }
    }
}