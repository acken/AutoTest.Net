using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;

namespace AutoTest.Runners.Shared
{
    public interface IAutoTestNetTestRunner
    {
        bool Handles(string identifier);
        IEnumerable<TestResult> Run(RunnerOptions options);
    }
}
