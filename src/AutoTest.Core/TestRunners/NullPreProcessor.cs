using System;
using System.Diagnostics;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
namespace AutoTest.Core.TestRunners
{
	class NullPreProcessor : IPreProcessTestruns
	{
		#region IPreProcessTestruns implementation
		public Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<ProcessStartInfo, bool>> FetchWrapper(Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<ProcessStartInfo, bool>> wrapper)
		{
			return wrapper;
		}

        public PreProcessedTesRuns PreProcess(PreProcessedTesRuns preProcessed)
		{
            return preProcessed;
		}

        public void RunFinished(TestRunResults[] results)
        {
        }
		#endregion
	}
}

