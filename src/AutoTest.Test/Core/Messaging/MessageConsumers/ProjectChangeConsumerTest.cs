using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.FileSystem;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Messaging;
using Rhino.Mocks;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Configuration;
using System.Reflection;
using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;
using System.IO;

namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
    [TestFixture]
    public class ProjectChangeConsumerTest
    {
        private ProjectChangeConsumer _consumer;
        private Project _project;
        private IMessageBus _bus;
        private IGenerateBuildList _listGenerator;
        private ICache _cache;
        private IConfiguration _configuration;
        private IBuildRunner _buildRunner;
        private ITestRunner _testRunner;

        [SetUp]
        public void SetUp()
        {
            _project = new Project(Path.GetFullPath("someProject.csproj"), new ProjectDocument(ProjectType.CSharp));
			_project.Value.SetOutputPath("");
			_project.Value.SetAssemblyName("someAssembly.dll");
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _listGenerator = MockRepository.GenerateMock<IGenerateBuildList>();
            _cache = MockRepository.GenerateMock<ICache>();
            _configuration = MockRepository.GenerateMock<IConfiguration>();
            _buildRunner = MockRepository.GenerateMock<IBuildRunner>();
            _testRunner = MockRepository.GenerateMock<ITestRunner>();
            _consumer = new ProjectChangeConsumer(_bus, _listGenerator, _cache, _configuration, _buildRunner, new ITestRunner[] { _testRunner });

            _cache.Stub(c => c.Get<Project>(null)).IgnoreArguments().Return(_project);
        }

        [Test]
        public void Should_be_a_blocking_consumer()
        {
            _consumer.ShouldBeOfType<IBlockingConsumerOf<ProjectChangeMessage>>();
        }

        [Test]
        public void Should_publish_run_started_message()
        {
            _listGenerator.Stub(l => l.Generate(null)).IgnoreArguments().Return(new string[] { });

            _consumer.Consume(new ProjectChangeMessage());
            _bus.AssertWasCalled(b => b.Publish<RunStartedMessage>(null), b => b.IgnoreArguments());
        }

        [Test]
        public void Should_publish_run_finished_message()
        {
            _listGenerator.Stub(l => l.Generate(null)).IgnoreArguments().Return(new string[] {});

            _consumer.Consume(new ProjectChangeMessage());
            _bus.AssertWasCalled(b => b.Publish<RunFinishedMessage>(null), b => b.IgnoreArguments());
        }

        [Test]
        public void Should_run_builds()
        {
            var executable = Assembly.GetExecutingAssembly().Location;
            _listGenerator.Stub(l => l.Generate(null)).IgnoreArguments().Return(new string[] { "some file.csproj" });
            _configuration.Stub(c => c.BuildExecutable(_project.Value)).Return(executable);
            _buildRunner.Stub(b => b.RunBuild(_project.Key, executable)).Return(new BuildRunResults(""));

            var message = new ProjectChangeMessage();
            message.AddFile(new ChangedFile("some file.csproj"));
            _consumer.Consume(message);
            _buildRunner.AssertWasCalled(b => b.RunBuild(_project.Key, executable));
        }

        [Test]
        public void Should_not_run_builds_when_build_executable_not_defined()
        {
            var executable = Assembly.GetExecutingAssembly().Location;
            _listGenerator.Stub(l => l.Generate(null)).IgnoreArguments().Return(new string[] { "some file.csproj" });
            _configuration.Stub(c => c.BuildExecutable(_project.Value)).Return("non existing file");

            var message = new ProjectChangeMessage();
            message.AddFile(new ChangedFile("some file.csproj"));
            _consumer.Consume(message);
            _buildRunner.AssertWasNotCalled(b => b.RunBuild(_project.Key, executable), b => b.IgnoreArguments());
        }

        [Test]
        public void Should_run_tests()
        {
            _project.Value.SetAsNUnitTestContainer();
            _project.Value.SetOutputPath("");
            _project.Value.SetAssemblyName("someProject.dll");
            _listGenerator.Stub(l => l.Generate(null)).IgnoreArguments().Return(new string[] { "some file.csproj" });
            _configuration.Stub(c => c.BuildExecutable(_project.Value)).Return("invalid_to_not_run_builds.exe");
            _testRunner.Stub(t => t.CanHandleTestFor(_project.Value)).Return(true);
            _testRunner.Stub(t => t.RunTests(new TestRunInfo[] { new TestRunInfo(_project, "") })).IgnoreArguments()
                .Return(new TestRunResults[] { new TestRunResults("", "", new TestResult[] {}) });

            var message = new ProjectChangeMessage();
            message.AddFile(new ChangedFile("some file.csproj"));
            _consumer.Consume(message);
            _testRunner.AssertWasCalled(t => t.RunTests(new TestRunInfo[] { new TestRunInfo(null, "") }), t => t.IgnoreArguments());
        }
    }
}
