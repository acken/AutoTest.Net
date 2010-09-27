using System;
using NUnit.Framework;
using AutoTest.Core;
using AutoTest.Core.Messaging;
using AutoTest.Core.FileSystem;
using Rhino.Mocks;
namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
	[TestFixture]
	public class BinaryFileChangeConsumerTest
	{
		private IMessageBus _bus;
		private BinaryFileChangeConsumer _consumer;
		private FileChangeMessage _files;
		
		[SetUp]
		public void SetUp()
		{
			_bus = MockRepository.GenerateMock<IMessageBus>();
			_consumer = new BinaryFileChangeConsumer(_bus);
			_files = new FileChangeMessage();
		}
		
		[Test]
		public void Should_not_consume_regular_file()
		{
			_files.AddFile(new ChangedFile("someTextFile.txt"));
			_consumer.Consume(_files);
			_bus.AssertWasNotCalled(b => b.Publish<AssemblyChangeMessage>(null), b => b.IgnoreArguments());
		}
		
		[Test]
		public void Should_consume_exe_files()
		{
			_files.AddFile(new ChangedFile("myexefile.exe"));
			_consumer.Consume(_files);
			_bus.AssertWasCalled(b => b.Publish<AssemblyChangeMessage>(null), b => b.IgnoreArguments());
		}
		
		[Test]
		public void Should_consume_dll_files()
		{
			_files.AddFile(new ChangedFile("mylibrary.dll"));
			_consumer.Consume(_files);
			_bus.AssertWasCalled(b => b.Publish<AssemblyChangeMessage>(null), b => b.IgnoreArguments());
		}
	}
}

