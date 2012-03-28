using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.QualityTools.UnitTestFramework
{
    [Serializable]
    public abstract class UnitTestAssertException : Exception
    {
        protected UnitTestAssertException() {}
        protected UnitTestAssertException(string msg) : base(msg) {}
        protected UnitTestAssertException(SerializationInfo info, StreamingContext context) : base(info, context) {}
        protected UnitTestAssertException(string msg, Exception ex) : base(msg, ex) {}
    }
}
