using System.IO;

namespace AutoTest.Core.FileSystem
{
    public interface IWatchValidator
    {
        bool ShouldPublish(string filePath);
		string GetIgnorePatterns();
    }
}