namespace AutoTest.Core.Messaging
{
    using System.IO;

    public class ProjectChangeMessage : FileChangeMessage
    {
        public ProjectChangeMessage(string fullFilePath, string name, string extension)
            : base(fullFilePath, name, extension)
        {
        }

        public ProjectChangeMessage(FileSystemInfo info) : base(info)
        {
        }
    }
}