using AutoTest.Core.Messaging;
using System.Threading;

namespace AutoTest.Test.TestObjects
{
    class StringMessage : IMessage
    {
        private object _padLock = new object();
        private int _timesConsumed = 0;

        public int TimesConsumed { get { return _timesConsumed; } }

        public void Consume()
        {
            lock (_padLock)
            {
                _timesConsumed++;
            }
        }
    }

    class IntMessage : IMessage { public bool Consumed { get; set; } }

    class BlockingMessage : IMessage { public bool Consumed { get; set; } }
    class BlockingMessage2 : IMessage { public bool Consumed { get; set; } }

    internal class Listener : IConsumerOf<StringMessage>
    {
        public void Consume(StringMessage message)
        {
            message.Consume();
        }
    }

    internal class BigListener : IConsumerOf<StringMessage>, IConsumerOf<IntMessage>
    {
        public void Consume(StringMessage message)
        {
            message.Consume();
        }

        public void Consume(IntMessage message)
        {
            message.Consumed = true;
        }
    }

    internal class BlockingConsumer : IBlockingConsumerOf<BlockingMessage>
    {
        public static int SleepTime { get; set; }

        #region IBlockingConsumerOf<BlockingMessage> Members

        public void Consume(BlockingMessage message)
        {
            message.Consumed = true;
            Thread.Sleep(SleepTime);
        }

        #endregion
    }

    internal class BlockingConsumer2 : IBlockingConsumerOf<BlockingMessage2>
    {
        #region IBlockingConsumerOf<BlockingMessage2> Members

        public void Consume(BlockingMessage2 message)
        {
            message.Consumed = true;
        }

        #endregion
    }
}