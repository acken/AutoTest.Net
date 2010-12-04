using AutoTest.Core.BuildRunners;
using AutoTest.Core.TestRunners;

namespace AutoTest.Core.Caching.RunResultCache
{
    public interface IRunResultCache
    {
        BuildItem[] Errors { get; }
        BuildItem[] Warnings { get; }
        TestItem[] Failed { get; }
        TestItem[] Ignored { get; }

        BuildItem[] AddedErrors { get; }
        BuildItem[] RemovedErrors { get; }

        TestItem[] AddedTests { get; }
        TestItem[] RemovedTests { get; }
    }
}