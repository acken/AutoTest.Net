using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared
{
    public interface IAutoTestNetTestRunner
    {
        bool Handles(string identifier);
        IEnumerable<TestResult> Run(RunnerOptions options);
    }
}
