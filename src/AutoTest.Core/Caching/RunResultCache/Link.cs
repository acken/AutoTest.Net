using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching.RunResultCache
{
    public class Link
    {
        private int _start;
        private int _length;

        public Link(int start, int length)
        {
            _start = start;
            _length = length;
        }

        public int Start { get { return _start; } }
        public int Length { get { return _length; } }
    }
}
