using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Core.BuildRunners;

namespace AutoTest.Core.Messaging
{
    public class BuildRunMessage : IMessage
    {
        private BuildRunResults _results;

        public BuildRunResults Results { get { return _results; } }

        public BuildRunMessage(BuildRunResults results)
        {
            _results = results;
        }
    }
}
