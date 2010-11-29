using System;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
namespace AutoTest.Core.TestRunners
{
	public interface IPreProcessTestruns
	{
        RunInfo[] PreProcess(RunInfo[] details);
        void RunFinished(TestRunResults[] results);
	}
}

