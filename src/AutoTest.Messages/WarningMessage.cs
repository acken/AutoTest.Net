using System;
namespace AutoTest.Messages
{
	public class WarningMessage : IMessage
    {
        public string Warning { get; private set; }

        public WarningMessage(string warning)
        {
            Warning = warning;
        }
    }
}

