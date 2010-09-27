using System;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Caching;
using AutoTest.Core.DebugLog;
namespace AutoTest.Core.Messaging
{
	class BinaryFileChangeConsumer : IConsumerOf<FileChangeMessage>
	{
		private IMessageBus _bus;
		
		public BinaryFileChangeConsumer(IMessageBus bus)
		{
			_bus = bus;
		}
		
		#region IConsumerOf[FileChangeMessage] implementation
		public void Consume(FileChangeMessage message)
		{
			Debug.WriteMessage("Consuming filechange through BinaryFileChangeConsumer");
			var assemblyChange = new AssemblyChangeMessage();
			foreach (var file in message.Files)
			{
				if (file.Extension.ToLower().Equals(".exe"))
					assemblyChange.AddFile(file);
				if (file.Extension.ToLower().Equals(".dll"))
					assemblyChange.AddFile(file);
			}
			if (assemblyChange.Files.Length > 0)
				_bus.Publish(assemblyChange);
		}
		#endregion
	}
}

