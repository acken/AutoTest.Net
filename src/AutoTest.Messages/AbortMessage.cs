using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTest.Messages
{
    public class AbortMessage : IMessage
    {
        public void WriteDataTo(BinaryWriter writer)
        {
        }

        public void SetDataFrom(BinaryReader reader)
        {
        }
    }
}
