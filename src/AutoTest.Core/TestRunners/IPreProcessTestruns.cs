using System;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
namespace AutoTest.Core.TestRunners
{
	public interface IPreProcessTestruns
	{
        void PreProcess(RunInfo[] details);
        void RunFinished(RunReport report);
	}
}

