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
		bool NotifyOnRunStarted { get; }
		bool NotifyOnRunCompleted { get; }
		string GrowlNotify { get; }
		string[] WatchIgnoreList { get; }
		bool ShouldUseIgnoreLists { get; }
		int FileChangeBatchDelay { get; }
		string[] TestAssembliesToIgnore { get; }
		string[] TestCategoriesToIgnore { get; }
		string CustomOutputPath { get; }
		bool RerunFailedTestsFirst { get; }

        void ValidateSettings();
		void BuildIgnoreListFromPath(string watchPath);
		void SetBuildProvider();
		void AnnounceTrackerType();
		void Merge(string configuratoinFile);
    }
}
