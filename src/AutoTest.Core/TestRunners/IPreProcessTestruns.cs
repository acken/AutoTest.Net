using System;
using AutoTest.Core.Messaging.MessageConsumers;
namespace AutoTest.Core.TestRunners
{
	public interface IPreProcessTestruns
	{
        void PreProcess(RunInfo[] details);
	}
}

