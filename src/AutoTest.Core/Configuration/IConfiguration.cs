using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Configuration
{
    public interface IConfiguration
    {
        string DirectoryToWatch { get; }
        string BuildExecutable { get; }
        string NunitTestRunner { get; }
        string MSTestRunner { get; }
        CodeEditor CodeEditor { get; }
        bool DebuggingEnabled { get; }

        void ValidateSettings();
    }
}
