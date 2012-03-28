using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.QualityTools.UnitTestFramework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ClassCleanupAttribute : Attribute
    {
    }
}
