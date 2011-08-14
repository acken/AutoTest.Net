using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Configuration;
using Castle.MicroKernel.Registration;
using AutoTest.Core.Presenters;
using AutoTest.Core.Messaging;
using AutoTest.Messages;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.Core.FileSystem;
using AutoTest.VSAddin.Listeners;
using AutoTest.Core.Caching;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.VSAddin.ATEngine
{
    public class Engine
    {
        private string _watchToken;
        private IDirectoryWatcher _watcher;
        private FeedbackWindow _window;

        public bool IsRunning
        { 
            get 
            {
                if (_watcher == null)
                    return false;
                return !_watcher.IsPaused;
            }
        }

        public Engine(FeedbackWindow window)
        {
            _window = window;
        }

        public void Bootstrap(string watchToken)
        {
            _watchToken = watchToken;
            BootStrapper.Configure();
            BootStrapper.Container
                .Register(Component.For<IMessageProxy>()
                                    .Forward<IRunFeedbackView>()
                                    .Forward<IInformationFeedbackView>()
                                    .Forward<IConsumerOf<AbortMessage>>()
                                    .ImplementedBy<MessageProxy>().LifeStyle.Singleton);

            BootStrapper.Services
                .Locate<IRunResultCache>().EnabledDeltas();
            BootStrapper.InitializeCache(_watchToken);
            BootStrapper.Services
                .Locate<IMessageProxy>()
                .SetMessageForwarder(new FeedbackListener(_window));

            _watcher = BootStrapper.Services.Locate<IDirectoryWatcher>();
            _watcher.Watch(_watchToken);
        }

        public void Pause()
        {
            _watcher.Pause();
            _window.SetText("Engine is paused and will not detect changes");
        }

        public void Resume()
        {
            if (_watcher.IsPaused)
                _watcher.Resume();
            _window.SetText("Engine is running and waiting for changes");
        }

        public void Shutdown()
        {
            _watcher.Dispose();
            BootStrapper.ShutDown();
        }

        public void BuildAndTestAll()
        {
            var message = new ProjectChangeMessage();
            var cache = BootStrapper.Services.Locate<ICache>();
            var bus = BootStrapper.Services.Locate<IMessageBus>();
            var projects = cache.GetAll<Project>();
            foreach (var project in projects)
            {
                if (project.Value == null)
                    continue;
                project.Value.RebuildOnNextRun();
                message.AddFile(new ChangedFile(project.Key));
            }
            bus.Publish(message);
        }
    }
}
