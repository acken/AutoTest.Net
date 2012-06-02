using System;
using System.Diagnostics;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
namespace AutoTest.Core.TestRunners
{
	public interface IPreProcessTestruns
	{
		Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<ProcessStartInfo, bool>> FetchWrapper(Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<ProcessStartInfo, bool>> wrapper);
        PreProcessedTesRuns PreProcess(PreProcessedTesRuns details);
        void RunFinished(TestRunResults[] results);
	}
}

