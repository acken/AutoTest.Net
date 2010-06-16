using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    class RunReport
    {
        public int NumberOfProjectsBuilt { get; set; }
        public int NumberOfTestsRan { get; set; }

        public RunReport()
        {
            NumberOfProjectsBuilt = 0;
            NumberOfTestsRan = 0;
        }
    }
}
