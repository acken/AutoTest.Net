using System;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Messages;
namespace AutoTest.Core.TestRunners
{
	class NullPreProcessor : IPreProcessTestruns
	{
		#region IPreProcessTestruns implementation
		public void PreProcess(RunInfo[] details)
		{
		}

        public void RunFinished(TestRunResults[] results)
        {
        }
		#endregion
	}
}

