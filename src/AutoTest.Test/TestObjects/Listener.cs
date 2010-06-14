using AutoTest.Core.Messaging;

namespace AutoTest.Test.TestObjects
{
    internal class Listener : IConsumerOf<string>
    {
        public static string LastMessage { get; private set; }

        public void Consume(string message)
        {
            LastMessage = message;
        }
    }

    internal class BigListener : IConsumerOf<string>, IConsumerOf<int>
    {
        public static string LastStringMessage { get; private set; }
        public static int LastIntMessage { get; private set; }

        public void Consume(string message)
        {
            LastStringMessage = message;
        }

        public void Consume(int message)
        {
            LastIntMessage = message;
        }
    }
}