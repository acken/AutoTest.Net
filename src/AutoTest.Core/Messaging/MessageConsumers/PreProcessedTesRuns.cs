using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    public class PreProcessedTesRuns
    {
        public Action<Action<ProcessStartInfo>> ProcessWrapper { get; private set; }
        public RunInfo[] RunInfos { get; private set; }

        public PreProcessedTesRuns(Action<Action<ProcessStartInfo>> processWrapper, RunInfo[] runinfos)
        {
            ProcessWrapper = processWrapper;
            RunInfos = runinfos;
        }
    }
}
