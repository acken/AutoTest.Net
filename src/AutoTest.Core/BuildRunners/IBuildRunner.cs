using AutoTest.Messages;
using AutoTest.Core.Caching.Projects;
using System;

namespace AutoTest.Core.BuildRunners
{
    public interface IBuildRunner
    {
        BuildRunResults RunBuild(string solution, bool rebuild, string buildExecutable, Func<bool> abortIfTrue);
        BuildRunResults RunBuild(Project project, string buildExecutable, Func<bool> abortIfTrue);
    }
}