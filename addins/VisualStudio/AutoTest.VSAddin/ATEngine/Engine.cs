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
using AutoTest.Core.TestRunners;
using AutoTest.Core.DebugLog;
using AutoTest.VS.Util.Debugger;
using EnvDTE80;
using System.IO;

namespace AutoTest.VSAddin.ATEngine
{
    public class Engine
    {
        private string _watchToken;
        private string _configuredCustomOutput;
        private IDirectoryWatcher _watcher;
        private FeedbackWindow _window;
        private DTE2 _application;

        public bool IsRunning
        { 
            get 
            {
                if (_watcher == null)
                    return false;
                return !_watcher.IsPaused;
            }
        }

        public List<OnDemandRun> LastTestRun { get; private set; }
        public CacheTestMessage LastDebugSession { get; private set; }

        public Engine(FeedbackWindow window, DTE2 application)
        {
            _window = window;
            _application = application;
        }

        public void SetLastTestRun(IEnumerable<OnDemandRun> runs)
        {
            LastTestRun = new List<OnDemandRun>(runs);
        }

        public void SetLastDebugSession(CacheTestMessage msg)
        {
            LastDebugSession = msg;
        }

        public void Bootstrap(string watchToken)
        {
            _watchToken = watchToken;
            BootStrapper.Configure(new LocalAppDataConfigurationLocator("AutoTest.config.template.VS"));
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

            _configuredCustomOutput = BootStrapper.Services.Locate<IConfiguration>().CustomOutputPath;
            _watcher = BootStrapper.Services.Locate<IDirectoryWatcher>();
            _watcher.Watch(_watchToken);
            _window.DebugTest += new EventHandler<UI.DebugTestArgs>(_window_DebugTest);
            _window.SetMessageBus(BootStrapper.Services.Locate<IMessageBus>());
            setCustomOutputPath();
            _window.Clear();
        }

        public void Pause()
        {
            _watcher.Pause();
            setCustomOutputPath();
            _window.SetText("Engine is paused and will not detect changes");
        }

        public void Resume()
        {
            if (_watcher.IsPaused)
                _watcher.Resume();
            setCustomOutputPath();
            _window.SetText("Engine is running and waiting for changes");
        }

        public void Shutdown()
        {
            _window.DebugTest -= _window_DebugTest;
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

        public void RerunLastTestRun(Action runOnTaskCompleted)
        {
            if (LastTestRun != null)
                RunTests(LastTestRun, runOnTaskCompleted);
        }

        public void RunTests(OnDemandRun run, Action runOnTaskCompleted)
        {
            RunTests(new List<OnDemandRun>(new OnDemandRun[] { run }), runOnTaskCompleted);
        }

        public void RunTests(List<OnDemandRun> runs, Action runOnTaskCompleted)
        {
            var cache = BootStrapper.Services.Locate<ICache>();
            var bus = BootStrapper.Services.Locate<IMessageBus>();
            var onDemandRunner = BootStrapper.Services.Locate<IOnDemanTestrunPreprocessor>();
            var projects = cache.GetAll<Project>();
            foreach (var run in runs)
                onDemandRunner.AddRuns(run);
            onDemandRunner.Activate(runOnTaskCompleted);
            LastTestRun = runs;
            bus.Publish(getOnDemandMessage(runs, projects));
        }

        private ProjectChangeMessage getOnDemandMessage(IEnumerable<OnDemandRun> runs, Project[] projects)
        {
            var message = new ProjectChangeMessage();
            foreach (var run in runs)
            {
                var project = projects.Where(x => x.Key.Equals(run.Project)).FirstOrDefault();
                if (project == null)
                {
                    Debug.WriteError(string.Format("Did not find matching project for run {0}", run.Project));
                    continue;
                }
                message.AddFile(new ChangedFile(run.Project));
            }
            return message;
        }

        public void RerunLastDebugSession()
        {
            if (LastDebugSession != null)
                DebugTest(LastDebugSession);
        }

        public void DebugTest(CacheTestMessage message)
        {
            var debugger = new DebugHandler(_application);
            debugger.Debug(message);
            LastDebugSession = message;
        }

        void _window_DebugTest(object sender, UI.DebugTestArgs e)
        {
            DebugTest(e.Test);
        }

        public string GetAssemblyFromProject(string projectPath)
        {
            var cache = BootStrapper.Services.Locate<ICache>();
            var config = BootStrapper.Services.Locate<IConfiguration>();
            var project = cache.Get<Project>(projectPath);
            if (project == null)
                return null;
            return project.GetAssembly(config.CustomOutputPath);
        }

        private void setCustomOutputPath()
        {
            if (!IsRunning)
                SetCustomOutputPath(Path.Combine("bin", "Debug"));
            else
                SetCustomOutputPath(_configuredCustomOutput);
        }

        public void SetCustomOutputPath(string newPath)
        {
            var config = BootStrapper.Services.Locate<IConfiguration>();
            if (config.CustomOutputPath.Equals(newPath))
                return;

            Debug.WriteDebug("Setting custom output folder to " + newPath);
            config.SetCustomOutputPath(newPath);
        }
    }
}
