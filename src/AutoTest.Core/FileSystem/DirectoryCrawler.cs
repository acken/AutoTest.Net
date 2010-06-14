namespace AutoTest.Core.FileSystem
{
    using System;
    using System.IO;
    using System.Linq;

    public interface IDirectoryCrawler
    {
        FileInfo FindParent(string startDirectory, Func<FileInfo, bool> predicate);
    }
    
    public class DirectoryCrawler : IDirectoryCrawler
    {
        public FileInfo FindParent(string startDirectory, Func<FileInfo, bool> predicate)
        {
            return Find(new DirectoryInfo(startDirectory), predicate);
        }

        static FileInfo Find(DirectoryInfo info, Func<FileInfo, bool> predicate)
        {
            if(info == null)
            {
                return null;
            }
            return info.GetFiles().FirstOrDefault(predicate) ?? Find(info.Parent, predicate);
        }
    }
}