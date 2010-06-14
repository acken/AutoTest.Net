using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching
{
    interface ICreate<T>
    {
        T Create(string key);
    }
}
