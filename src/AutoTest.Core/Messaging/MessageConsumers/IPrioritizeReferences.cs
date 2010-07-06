using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    interface IPrioritizeReferences
    {
        string[] Prioritize(string[] references);
    }
}
