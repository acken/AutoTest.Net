using AutoTest.Messages;
using AutoTest.Core.Caching.Projects;

namespace AutoTest.Core.BuildRunners
{
    public interface IBuildRunner
    {
        BuildRunResults RunBuild(string solution, bool rebuild, string buildExecutable);
        BuildRunResults RunBuild(Project project, string buildExecutable);
    }
}