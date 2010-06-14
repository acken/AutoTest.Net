using System.IO;
using log4net;
using AutoTest.Core.Messaging;
using AutoTest.Core.Configuration;
using AutoTest.Core.FileSystem.ProjectLocators;

namespace AutoTest.Core.FileSystem
{
    public class FileChangeConsumer : IConsumerOf<FileChangeMessage>
    {
        private readonly IServiceLocator _services;
        private readonly IMessageBus _bus;
        private static readonly ILog _logger = LogManager.GetLogger(typeof (FileChangeConsumer));

        public FileChangeConsumer(IServiceLocator services, IMessageBus bus)
        {
            _services = services;
            _bus = bus;
        }

        public void Consume(FileChangeMessage message)
        {
            var projectChange = new ProjectChangeMessage();
            var locators = _services.LocateAll<ILocateProjects>();
            foreach (var file in message.Files)
            {
                _logger.DebugFormat("Project Locator found a signification change in file \"{0}\". Publishing {1}.", file.Name, typeof(ProjectChangeMessage).Name);
                foreach (var locator in locators)
                {
                    var files = locator.Locate(file.FullName);
                    projectChange.AddFile(files);
                }   
            }
            if (projectChange.Files.Length > 0)
                _bus.Publish(projectChange);
        }
    }
}