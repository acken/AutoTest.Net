using System;
using NUnit.Framework;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.TestRunners;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using Rhino.Mocks;
using AutoTest.Messages;
namespace AutoTest.Test
{
	[TestFixture]
	public class AssemblyChangeConsumerTest
	{
		private AssemblyChangeConsumer _consumer;
        private IMessageBus _bus;
        private ITestRunner _testRunner;
		private IDetermineIfAssemblyShouldBeTested _testAssemblyValidator;

        [SetUp]
        public void SetUp()
        {
            _bus = MockRepository.GenerateMock<IMessageBus>();
            _testRunner = MockRepository.GenerateMock<ITestRunner>();
			_testAssemblyValidator = MockRepository.GenerateMock<IDetermineIfAssemblyShouldBeTested>();
            _consumer = new AssemblyChangeConsumer(new ITestRunner[] { _testRunner }, _bus, _testAssemblyValidator);
			_testRunner.Stub(r => r.RunTests(null)).IgnoreArguments().Return(new TestRunResults[] {});
        }
		
		[Test]
		public void Should_run_tests()
		{
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly("")).Return(false);
			_testRunner.Stub(t => t.CanHandleTestFor(new ChangedFile())).IgnoreArguments().Return(true);
			var message = new AssemblyChangeMessage();
			message.AddFile(new ChangedFile());
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly(null)).IgnoreArguments().Return(true);
			_consumer.Consume(message);
			_testRunner.AssertWasCalled(t => t.RunTests(null), t => t.IgnoreArguments());
		}
		
		[Test]
		public void Should_not_run_tests_for_assemblies_that_runner_doesnt_support()
		{
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly("")).Return(false);
			_testRunner.Stub(t => t.CanHandleTestFor(new ChangedFile())).IgnoreArguments().Return(false);
			var message = new AssemblyChangeMessage();
			message.AddFile(new ChangedFile());
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly(null)).IgnoreArguments().Return(true);
			_consumer.Consume(message);
			_testRunner.AssertWasNotCalled(t => t.RunTests(null), t => t.IgnoreArguments());
		}
		
		[Test]
		public void Should_ignore_test_assembly()
		{
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly("")).Return(true);
			_testRunner.Stub(t => t.CanHandleTestFor(new ChangedFile())).IgnoreArguments().Return(true);
			var message = new AssemblyChangeMessage();
			message.AddFile(new ChangedFile());
			_testAssemblyValidator.Stub(t => t.ShouldNotTestAssembly(null)).IgnoreArguments().Return(true);
			_consumer.Consume(message);
			_testRunner.AssertWasNotCalled(t => t.RunTests(null), t => t.IgnoreArguments());
		}
	}
}

