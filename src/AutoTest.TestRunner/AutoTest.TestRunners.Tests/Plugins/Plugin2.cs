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

        public bool IsTest(string assembly, string type)
        {
            return true;
        }

        public bool ContainsTests(string assembly, string type)
        {
            return true;
        }

        bool Shared.IAutoTestNetTestRunner.Handles(string identifier)
        {
            return true;
        }

        IEnumerable<Shared.Results.TestResult> Shared.IAutoTestNetTestRunner.Run(Shared.Options.RunnerOptions options)
        {
            return null;
        }
    }
}
