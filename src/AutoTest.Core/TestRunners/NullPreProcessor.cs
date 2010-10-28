using System;
namespace AutoTest.Core.TestRunners
{
	class NullPreProcessor : IPreProcessTestruns
	{
		#region IPreProcessTestruns implementation
		public void PreProcess(TestRunDetails[] details)
		{
		}
		#endregion
	}
}

