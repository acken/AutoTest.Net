using System;
using System.Collections.Generic;
using System.Linq;

using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Configuration;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;

namespace AutoTest.Core.TestRunners.TestRunners
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> ContinueWith<T>(this IEnumerable<T> source1, Func<IEnumerable<T>> source2)
        {
            return source1.Any()
                       ? source1
                       : source2();
        }
    }

    public class MSpecTestRunner : ITestRunner
    {
        readonly IMSpecCommandLineBuilder _commandLineBuilder;
        readonly IConfiguration _configuration;
        readonly IExternalProcess _externalProcess;
        readonly IFileSystemService _fileSystem;
        readonly IResolveAssemblyReferences _referenceResolver;

        public MSpecTestRunner(IResolveAssemblyReferences referenceResolver,
                               IConfiguration configuration,
                               IFileSystemService fileSystem,
                               IExternalProcess externalProcess,
                               IMSpecCommandLineBuilder commandLineBuilder)
        {
            _referenceResolver = referenceResolver;
            _configuration = configuration;
            _fileSystem = fileSystem;
            _externalProcess = externalProcess;
            _commandLineBuilder = commandLineBuilder;
        }

        public bool CanHandleTestFor(ProjectDocument document)
        {
            return document.ContainsMSpecTests;
        }

        public bool CanHandleTestFor(string assembly)
        {
            var references = _referenceResolver.GetReferences(assembly);
            return references.Contains("machine.specifications");
        }

        public TestRunResults[] RunTests(TestRunInfo[] runInfos)
        {
            return runInfos
                .GroupBy(Framework)
                .Select(x => new Run
                             {
                                 RunnerExe = _configuration.MSpecTestRunner(x.Key),
                                 RunInfos = x.AsEnumerable()
                             })
                .Select(x => RunnerExeExists(x).ContinueWith(() => RunTestsForFramework(x)))
                .SelectMany(x => x)
                .ToArray();
        }

        static string Framework(TestRunInfo runInfo)
        {
            return runInfo.Project == null ? String.Empty : runInfo.Project.Value.Framework;
        }

        IEnumerable<TestRunResults> RunnerExeExists(Run run)
        {
            if (_fileSystem.FileExists(run.RunnerExe))
            {
                return Enumerable.Empty<TestRunResults>();
            }

            return ErrorsFor(run.RunInfos);
        }

        static IEnumerable<TestRunResults> ErrorsFor(IEnumerable<TestRunInfo> runInfo)
        {
            return runInfo
                .Select(x => new
                             {
                                 Project = x.Project != null ? x.Project.Key : String.Empty,
                                 x.Assembly
                             })
                .Select(x => new TestRunResults(x.Project,
                                                x.Assembly,
                                                false,
                                                TestRunner.MSpec,
                                                new TestResult[] { }));
        }

        IEnumerable<TestRunResults> RunTestsForFramework(Run run)
        {
            try
            {
                var runnerExe = run.RunnerExe;
                var arguments = _commandLineBuilder.Build(run);

                _externalProcess.CreateAndWaitForExit(runnerExe, arguments);

                //                var parser = new NUnitTestResponseParser(_bus, TestRunner.MSpec);
                //                using (TextReader reader = new StreamReader(report))
                //                {
                //                    parser.Parse(reader.ReadToEnd(), runInfos, false);
                //                }
                //
                //                foreach (var result in parser.Result)
                //                {
                //                    results.Add(result);
                //                }
                //
                //                return results.ToArray();
            }
            finally
            {
                run.Cleanup();
            }

            yield break;
        }

        public class Run
        {
            readonly List<Action> _cleanups = new List<Action>();

            public string RunnerExe
            {
                get;
                set;
            }

            public IEnumerable<TestRunInfo> RunInfos
            {
                get;
                set;
            }

            public IEnumerable<Action> Cleanups
            {
                get { return _cleanups; }
            }

            public void RegisterCleanup(Action cleanup)
            {
                _cleanups.Add(cleanup);
            }

            public void Cleanup()
            {
                Cleanups
                    .AsEnumerable()
                    .Reverse()
                    .ToList()
                    .ForEach(x => x());
            }
        }
    }
}