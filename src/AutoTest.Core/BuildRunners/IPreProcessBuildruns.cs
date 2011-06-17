using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.Messaging.MessageConsumers;

namespace AutoTest.Core.BuildRunners
{
    public interface IPreProcessBuildruns
    {
        RunInfo[] PreProcess(RunInfo[] details);
        RunInfo[] PostProcess(RunInfo[] details);
    }
}
