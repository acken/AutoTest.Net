using System;
namespace AutoTest.Messages
{
	[Serializable]
	public class TestRunMessage : IMessage
    {
        private TestRunResults _results;

        public TestRunResults Results { get { return _results; } }

        public TestRunMessage(TestRunResults results)
        {
            _results = results;
        }
    }
}

