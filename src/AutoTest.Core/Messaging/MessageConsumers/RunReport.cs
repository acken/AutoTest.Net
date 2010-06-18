using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class RunReport
    {
        public int NumberOfBuildsSucceeded { get; set; }
        public int NumberOfBuildsFailed { get; set; }
        public int NumberOfProjectsBuilt { get { return NumberOfBuildsSucceeded + NumberOfBuildsFailed; } }
        public int NumberOfTestsPassed { get; set; }
        public int NumberOfTestsFailed { get; set; }
        public int NumberOfTestsIgnored { get; set; }
        public int NumberOfTestsRan { get { return NumberOfTestsPassed + NumberOfTestsFailed + NumberOfTestsIgnored; } }

        public RunReport()
        {
            NumberOfBuildsSucceeded = 0;
            NumberOfBuildsFailed = 0;
            NumberOfTestsPassed = 0;
            NumberOfTestsFailed = 0;
            NumberOfTestsIgnored = 0;
        }
    }
}
