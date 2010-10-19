using System;
namespace AutoTest.Messages
{
	public class RunFinishedMessage : IMessage
	{
		public RunReport Report { get; private set; }

        public RunFinishedMessage(RunReport report) 
        {
            Report = report;
        }
	}
}

