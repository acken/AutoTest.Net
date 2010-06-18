using System.Timers;
using System.Collections.Generic;
using AutoTest.Core.Messaging;
using System.IO;

namespace AutoTest.Core.FileSystem
{
    public class DirectoryWatcher : IDirectoryWatcher
    {
        private readonly IMessageBus _bus;
        private readonly FileSystemWatcher _watcher;
        private readonly System.Timers.Timer _batchTimer;
        private bool _timerIsRunning = false;
        private List<ChangedFile> _buffer = new List<ChangedFile>();
        private object _padLock = new object();
        private IWatchValidator _validator;

        public DirectoryWatcher(IMessageBus bus, IWatchValidator validator)
        {
            _bus = bus;
            _validator = validator;
            _batchTimer = new Timer(100);
            _batchTimer.Enabled = true;
            _batchTimer.Elapsed += _batchTimer_Elapsed;
            _watcher = new FileSystemWatcher
                           {
                               NotifyFilter = NotifyFilters.LastWrite,
                               IncludeSubdirectories = true,
                               Filter = "*.*",
                           };
            
            _watcher.Changed += WatcherChangeHandler;
            _watcher.Created += WatcherChangeHandler;
        }

        public void Watch(string path)
        {
            _watcher.Path = path;
            _watcher.EnableRaisingEvents = true;
        }

        private void WatcherChangeHandler(object sender, FileSystemEventArgs e)
        {
            addToBuffer(new ChangedFile(e.FullPath));
        }

        private void _batchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_padLock)
            {
                if (_buffer.Count > 0)
                {
                    var fileChange = new FileChangeMessage();
                    fileChange.AddFile(_buffer.ToArray());
                    _bus.Publish(fileChange);
                }
                _buffer.Clear();
                stopTimer();
            }
        }

        private void addToBuffer(ChangedFile file)
        {
            if (!_validator.ShouldPublish(file.FullName))
                return;

            lock (_padLock)
            {
                if (_buffer.FindIndex(0, f => f.FullName.Equals(file.FullName)) < 0)
                {
                    _buffer.Add(file);
                    reStartTimer();
                }
            }
        }

        private void reStartTimer()
        {
            stopTimer();
            startTimer();
        }

        private void startTimer()
        {
            if (!_timerIsRunning)
            {
                _batchTimer.Start();
                _timerIsRunning = true;
            }
        }

        private void stopTimer()
        {
            _batchTimer.Stop();
            _timerIsRunning = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}