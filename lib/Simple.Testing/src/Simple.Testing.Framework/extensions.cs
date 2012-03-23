using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Simple.Testing.ClientFramework;

namespace Simple.Testing.Framework
{
    public static class SpecificationExtensions
    {                                                     
        public static SpecificationToRun AsRunnable(this Specification specification)
        {
            return new SpecificationToRun(specification, null);
        }
    }
    static class DelegateExtensions
    {
        public static object InvokeIfNotNull(this Delegate d)
        {
            return d != null ? d.DynamicInvoke() : null;
        }
    } 

    public static class IEnuerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence) action(item);
        }
    }

    public static class MethodInfoExtensions
    {
        public static object CallMethod(this MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().Length > 0) return null;
            var obj = Activator.CreateInstance(methodInfo.DeclaringType);
            var ret = methodInfo.Invoke(obj, null);
            return ret;
        }
    }
}
