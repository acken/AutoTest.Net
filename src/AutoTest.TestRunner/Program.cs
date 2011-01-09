using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new RunOptions();
            options.AddRunner(new RunnerOptions());
            var runner = new NUnitProxy.NUnitRunner();
            runner.Initialize();
            runner.Execute(options.Runners.First());
        }
    }
}
