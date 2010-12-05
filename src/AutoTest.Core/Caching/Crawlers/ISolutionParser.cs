using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching.Crawlers
{
    interface ISolutionParser
    {
        void Crawl(string solutionFile);
    }
}
