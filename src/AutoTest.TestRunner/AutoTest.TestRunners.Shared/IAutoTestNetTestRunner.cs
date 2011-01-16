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
        void SetLogger(ILogger logger);

        bool Handles(string identifier);
        IEnumerable<TestResult> Run(RunnerOptions options);
    }
}
