using System.Timers;

namespace AutoTest.Core.FileSystem
{
    using System.IO;
    using Messaging;
using System.Collections.Generic;

    public class DirectoryWatcher : IDirectoryWatcher
    {
        private readonly IMessageBus _bus;
        private readonly FileSystemWatcher _watcher;
        private readonly System.Timers.Timer _batchTimer;
        private bool _timerIsRunning = false;
        private List<FileInfo> _buffer = new List<FileInfo>();
        private object _padLock = new object();
        private IWatchValidator _validator;

        public DirectoryWatcher(IMessageBus bus, IWatchValidator validator)
        {
            _bus = bus;
            _validator = validator;
            _batchTimer = new Timer(30);
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
            addToBuffer(new FileInfo(e.FullPath));
        }

        private void _batchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_padLock)
            {
                foreach (var file in _buffer)
                    _bus.Publish(new FileChangeMessage(file));
                _buffer.Clear();
                _batchTimer.Stop();
                _timerIsRunning = false;
            }
        }

        private void addToBuffer(FileInfo file)
        {
            if (!_validator.ShouldPublish(file.FullName))
                return;

            lock (_padLock)
            {
                if (_buffer.FindIndex(0, f => f.FullName.Equals(file.FullName)) < 0)
                {
                    _buffer.Add(file);
                    if (!_timerIsRunning)
                    {
                        _batchTimer.Start();
                        _timerIsRunning = true;
                    }
                }
            }
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