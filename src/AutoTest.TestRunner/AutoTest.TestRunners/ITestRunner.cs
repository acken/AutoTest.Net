using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared;
using AutoTest.TestRunners.Shared.Plugins;

namespace AutoTest.TestRunners
{
    interface ITestRunner
    {
        IEnumerable<TestResult> Run(Plugin plugin, RunOptions options);
    }
}
