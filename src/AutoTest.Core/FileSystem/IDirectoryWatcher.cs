namespace AutoTest.Core.FileSystem
{
    using System;

    public interface IDirectoryWatcher : IDisposable
    {
        void Pause();
        void Resume();
        void Watch(string path);
    }
}