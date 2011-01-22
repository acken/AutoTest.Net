using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Logging;

namespace AutoTest.TestRunners.Shared
{
    public interface IAutoTestNetTestRunner
    {
        string Identifier { get; }

        void SetLogger(ILogger logger);

        bool IsTest(string assembly, string member);
        bool ContainsTests(string assembly, string member);

        bool Handles(string identifier);
        IEnumerable<TestResult> Run(RunnerOptions options);
    }
}
