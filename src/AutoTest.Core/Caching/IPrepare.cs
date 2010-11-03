using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching
{
    interface IPrepare<T>
    {
        T Prepare(T record);
    }
}
