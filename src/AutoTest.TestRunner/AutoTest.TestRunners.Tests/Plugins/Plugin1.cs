using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Tests.Plugins
{
    public class Plugin1 : AutoTest.TestRunners.Shared.IAutoTestNetTestRunner
    {
        public bool Handles(string identifier)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Shared.Results.TestResult> Run(Shared.Options.RunnerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
