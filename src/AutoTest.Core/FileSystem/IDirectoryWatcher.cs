namespace AutoTest.Core.FileSystem
{
    using System;

    public interface IDirectoryWatcher : IDisposable
    {
        void Watch(string path);
    }
}