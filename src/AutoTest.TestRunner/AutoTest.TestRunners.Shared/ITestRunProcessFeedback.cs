using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared
{
    public interface ITestRunProcessFeedback
    {
        void ProcessStart(string commandline);
    }
}
