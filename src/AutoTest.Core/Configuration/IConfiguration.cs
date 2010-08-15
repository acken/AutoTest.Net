using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.Configuration
{
    public interface IConfiguration
    {
        string[] WatchDirectores { get; }
        string BuildExecutable(ProjectDocument project);
        string NunitTestRunner(string version);
        string MSTestRunner(string version);
        string XunitTestRunner(string version);
        CodeEditor CodeEditor { get; }
        bool DebuggingEnabled { get; }

        void ValidateSettings();
    }
}
