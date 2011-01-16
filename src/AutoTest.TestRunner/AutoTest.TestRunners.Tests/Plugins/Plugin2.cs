using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Logging;

namespace AutoTest.TestRunners.Tests.Plugins
{
    public class Plugin2 : AutoTest.TestRunners.Shared.IAutoTestNetTestRunner
    {
        public void SetLogger(ILogger logger)
        {
        }

        bool Shared.IAutoTestNetTestRunner.Handles(string identifier)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Shared.Results.TestResult> Shared.IAutoTestNetTestRunner.Run(Shared.Options.RunnerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
