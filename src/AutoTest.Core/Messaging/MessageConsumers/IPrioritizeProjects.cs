using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Messaging.MessageConsumers
{
    interface IPrioritizeProjects
    {
        string[] Prioritize(string[] references);
    }
}
