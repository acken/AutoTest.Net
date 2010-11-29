using System;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
namespace AutoTest.Core.TestRunners
{
	class NullPreProcessor : IPreProcessTestruns
	{
		#region IPreProcessTestruns implementation
        public RunInfo[] PreProcess(RunInfo[] details)
		{
            return details;
		}

        public void RunFinished(TestRunResults[] results)
        {
        }
		#endregion
	}
}

