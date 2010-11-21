using AutoTest.Messages;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.BuildRunners
{
    public interface IBuildRunner
    {
        BuildRunResults RunBuild(Project project, string buildExecutable);
    }
}