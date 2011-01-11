using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared;

namespace AutoTest.TestRunners.Tests.Plugins
{
    public class Plugin1 : IAutoTestNetTestRunner
    {
        public bool Handles(string identifier)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestResult> Run(Shared.RunnerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
