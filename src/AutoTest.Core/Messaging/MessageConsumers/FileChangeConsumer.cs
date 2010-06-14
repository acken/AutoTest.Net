using System;
using System.IO;
using log4net;
using AutoTest.Core.Messaging;
using AutoTest.Core.Configuration;
using AutoTest.Core.FileSystem.ProjectLocators;
using System.Collections.Generic;
using AutoTest.Core.FileSystem;

namespace AutoTest.Core.Messaging.MessageConsumers
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
            var totalListOfProjects = new List<ChangedFile>();
            var locators = _services.LocateAll<ILocateProjects>();
            foreach (var file in message.Files)
            {
                var projects = getProjectsClosestToChangedFile(file, locators);
                combineLists(projects, totalListOfProjects);
            }
            publishProjects(totalListOfProjects);
        }

        private void publishProjects(List<ChangedFile> totalListOfProjects)
        {
            if (totalListOfProjects.Count == 0)
                return;
            var projectChange = new ProjectChangeMessage();
            projectChange.AddFile(totalListOfProjects.ToArray());
            _bus.Publish(projectChange);
        }

        private ChangedFile[] getProjectsClosestToChangedFile(ChangedFile file, ILocateProjects[] locators)
        {
            var closestProjects = new List<ChangedFile>();
            var currentLocation = 0;
            _logger.DebugFormat("Project Locator found a signification change in file \"{0}\". Publishing {1}.", file.Name, typeof(ProjectChangeMessage).Name);
            foreach (var locator in locators)
            {
                var files = locator.Locate(file.FullName);
                if (files.Length == 0)
                    continue;
                currentLocation = addIfCloser(files, currentLocation, closestProjects);
            }
            return closestProjects.ToArray();
        }

        private int addIfCloser(ChangedFile[] suggestedProjects, int currentLocation, List<ChangedFile> closestProjects)
        {
            int location = getLocation(suggestedProjects[0]);
            if (isFertherAwayThanCurrent(location, currentLocation))
                return currentLocation;
            if (isCloserToChangedFile(location, currentLocation))
            {
                closestProjects.Clear();
                currentLocation = location;
            }
            combineLists(suggestedProjects, closestProjects);
            return currentLocation;
        }

        private bool isFertherAwayThanCurrent(int location, int currentLocation)
        {
            return location < currentLocation;
        }

        private int getLocation(ChangedFile suggestedProjects)
        {
            return suggestedProjects.FullName.Split(Path.DirectorySeparatorChar).Length;
        }

        private bool isCloserToChangedFile(int location, int currentLocation)
        {
            return location > currentLocation;
        }

        private void combineLists(ChangedFile[] source, List<ChangedFile> destination)
        {
            foreach (var changedFile in source)
                addUnique(destination, changedFile);
        }

        private void addUnique(List<ChangedFile> projectFiles, ChangedFile changedProject)
        {
            if (projectFiles.Find(x => x.FullName.Equals(changedProject.FullName)) == null)
                projectFiles.Add(changedProject);
        }
    }
}