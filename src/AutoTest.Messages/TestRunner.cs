using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Messages
{
    public enum TestRunner
    {
        Any = 0,
        NUnit = 1,
        MSTest = 2,
        XUnit = 3,
        MSpec = 4
    }

    public static class TestRunnerConverter
    {
        public static string ToString(TestRunner runner)
        {
            return runner.ToString();
        }

        public static TestRunner FromString(string runner)
        {
            return (TestRunner)Enum.Parse(typeof(TestRunner), runner, true);
        }
    }
}
