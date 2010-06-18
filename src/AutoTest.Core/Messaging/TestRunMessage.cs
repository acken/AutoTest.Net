using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.TestRunners;

namespace AutoTest.Core.Messaging
{
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
