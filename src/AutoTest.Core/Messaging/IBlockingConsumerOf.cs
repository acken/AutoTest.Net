using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging
{
    public interface IBlockingConsumerOf<TMesssage> : IMessageConsumer
    {
        void Consume(TMesssage message);
    }
}
