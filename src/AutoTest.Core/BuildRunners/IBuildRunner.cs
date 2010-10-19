using AutoTest.Messages;

namespace AutoTest.Core.BuildRunners
{
    public interface IBuildRunner
    {
        BuildRunResults RunBuild(string projectName, string buildExecutable);
    }
}