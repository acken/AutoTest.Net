using System;
using System.Collections.Generic;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Options;
namespace AutoTest.TestRunners.Shared
{
    public interface ITestRunProcess
    {
        TestSession Prepare(RunOptions options);
    }
}
