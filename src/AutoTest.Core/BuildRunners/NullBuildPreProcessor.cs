using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.Core.BuildRunners
{
    class NullBuildPreProcessor : IPreProcessBuildruns
    {
        public RunInfo[] PreProcess(RunInfo[] details)
        {
            return details;
        }

        public RunInfo[] PostProcess(RunInfo[] details)
        {
            return details;
        }
    }
}
