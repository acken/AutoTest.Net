using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging
{
    interface IBlockingConsumerOf<TMesssage> : IMessageConsumer
    {
        void Consume(TMesssage message);
    }
}
