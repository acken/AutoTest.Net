using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class PreProcessedTesRuns
    {
        public RunInfo[] RunInfos { get; private set; }

        public PreProcessedTesRuns(RunInfo[] runinfos)
        {
            RunInfos = runinfos;
        }
    }
}
