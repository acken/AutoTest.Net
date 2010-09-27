using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching.RunResultCache
{
    public class Link
    {
		public int Start { get; private set; }
		public int Length { get; private set; }

        public Link(int start, int length)
        {
            Start = start;
            Length = length;
        }

    }
}
