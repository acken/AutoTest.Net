using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Core.FileSystem
{
    class WatchValidator : IWatchValidator
    {
        private const string BIN_DEBUG = "bin{0}debug";
        private const string BIN_RELEASE = "bin{0}release";
        private const string BIN_X86 = "bin{0}x86";
        private const string OBJ_DEBUG = "obj{0}debug";
        private const string OBJ_RELEASE = "obj{0}release";
        private const string OBJ_X86 = "obj{0}x86";
        private const string OBJ = "obj{0}";
        private const string MONO_FILE_LIST_ABSOLUTE = ".FileListAbsolute.txt";
        private const string MONO_FILES_WRITTEN_ABSOLUTE = ".FilesWrittenAbsolute.txt";

        public bool ShouldPublish(string filePath)
        {
            if (contains(filePath, BIN_DEBUG))
                return false;
            if (contains(filePath, BIN_RELEASE))
                return false;
            if (contains(filePath, BIN_X86))
                return false;
            if (contains(filePath, OBJ_DEBUG))
                return false;
            if (contains(filePath, OBJ_RELEASE))
                return false;
            if (contains(filePath, OBJ_X86))
                return false;
            if (Directory.Exists(filePath))
                return false;
            if (contains(filePath, OBJ) && filePath.EndsWith(MONO_FILE_LIST_ABSOLUTE))
                return false;
            if (contains(filePath, OBJ) && filePath.EndsWith(MONO_FILES_WRITTEN_ABSOLUTE))
                return false;
            return true;
        }

        private bool contains(string path, string stringToSearchFor)
        {
            return path.IndexOf(preparePath(stringToSearchFor), StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private string preparePath(string path)
        {
            return string.Format(path, Path.DirectorySeparatorChar);
        }
    }
}
