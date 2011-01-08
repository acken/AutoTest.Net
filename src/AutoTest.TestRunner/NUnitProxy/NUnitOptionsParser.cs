using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunner.NUnitProxy
{
    class NUnitOptionsParser
    {
        private RunnerOptions _options;

        public string Tests { get; private set; }
        public string Categories { get; private set; }
        public string Assemblies { get; private set; }

        public NUnitOptionsParser(RunnerOptions options)
        {
            _options = options;
        }

        public NUnitOptionsParser Parse()
        {
            throw new NotImplementedException();
        }
    }
}
