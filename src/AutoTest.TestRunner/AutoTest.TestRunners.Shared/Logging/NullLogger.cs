﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.Logging
{
    public class NullLogger : ILogger
    {
        public void Write(string message)
        {
        }

        public void Write(string message, params object[] args)
        {
        }

        public void Write(Exception ex)
        {
        }
    }
}
