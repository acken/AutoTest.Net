using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Results;

namespace AutoTest.TestRunners.Shared.Errors
{
    public class ErrorHandler
    {
        public static TestResult GetError(Exception ex)
        {
            return GetError(ex.ToString());
        }

        public static TestResult GetError(string message)
        {
            return GetError("", message);
        }

        public static TestResult GetError(string runner, Exception ex)
        {
            return GetError(runner, ex.ToString());
        }

        public static TestResult GetError(string runner, string message)
        {
            return new TestResult(runner, "", "", 0, "AutoTest.TestRunner.exe internal error", TestState.Panic, message);
        }
    }
}
