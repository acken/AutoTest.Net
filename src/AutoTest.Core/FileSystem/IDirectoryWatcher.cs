namespace AutoTest.Core.FileSystem
{
    using System;

    public interface IDirectoryWatcher : IDisposable
    {
        bool IsPaused { get; }

        void Pause();
        void Resume();
        void Watch(string path);
    }
}