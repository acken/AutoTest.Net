using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.BuildRunners
{
    public class BuildMessage
    {
        public string File { get; set; }
        public int LineNumber { get; set; }
        public int LinePosition { get; set; }
        public string ErrorMessage { get; set; }
    }
}
