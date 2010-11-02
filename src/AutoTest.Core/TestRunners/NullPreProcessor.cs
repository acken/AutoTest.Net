using System;
using AutoTest.Core.Messaging.MessageConsumers;
namespace AutoTest.Core.TestRunners
{
	class NullPreProcessor : IPreProcessTestruns
	{
		#region IPreProcessTestruns implementation
		public void PreProcess(RunInfo[] details)
		{
		}
		#endregion
	}
}

