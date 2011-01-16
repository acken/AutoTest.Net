using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.Logging
{
    public interface ILogger
    {
        void Write(string message);
        void Write(Exception ex);
    }
}
